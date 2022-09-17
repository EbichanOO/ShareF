using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Mono.Cecil;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace PretiaArCloud.Networking.Weaver
{
    internal class Weaver
    {
        internal static void OnCompilationFinished(string assemblyPath, CompilerMessage[] compilerMessages)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(assemblyPath);
            if (fileNameWithoutExtension.Equals("PretiaSDK.Runtime")
            || fileNameWithoutExtension.Equals("PretiaSDK.Editor")
            || assemblyPath.Contains("Unity"))
            {
                return;
            }

            var compiledAssemblyReferences = Prepare(assemblyPath);
            WeaveDLL(assemblyPath, compiledAssemblyReferences);
        }

        private static HashSet<string> Prepare(string assemblyPath)
        {
            HashSet<string> compiledAssemblyReferences = null;

            foreach (var asm in CompilationPipeline.GetAssemblies())
            {
                if (!asm.outputPath.Equals(assemblyPath)) continue;

                var distinctCompiledPaths = asm.compiledAssemblyReferences
                    .Select(path => Path.GetDirectoryName(path))
                    .Distinct();

                compiledAssemblyReferences = new HashSet<string>(distinctCompiledPaths);
                compiledAssemblyReferences.Add(assemblyPath);
            }

            return compiledAssemblyReferences;
        }

        private static void WeaveDLL(string assemblyPath, IEnumerable<string> compiledAssemblyReferences)
        {
            string assemblyDirectory = Path.GetDirectoryName(assemblyPath);
            using (var asmResolver = new DefaultAssemblyResolver())
            {
                asmResolver.AddSearchDirectory(assemblyDirectory);
                if (compiledAssemblyReferences != null)
                {
                    foreach (var directory in compiledAssemblyReferences)
                    {
                        asmResolver.AddSearchDirectory(directory);
                    }
                }

                ReaderParameters readerParameters = new ReaderParameters
                {
                    ReadWrite = true,
                    ReadSymbols = true,
                    AssemblyResolver = asmResolver
                };

                List<TypeDefinition> networkMessages = default;
                List<TypeDefinition> messagePackObjectTypes = default;
                List<TypeDefinition> unionTypes = default;

                var sw = new System.Diagnostics.Stopwatch();
                sw.Start();

                WeaverPreProcessor.Execute(assemblyPath, readerParameters,
                    new PreProcess[] {
                        new PreProcess {
                            Process = NetworkMessageAttributeProcessor.GetNetworkMessages,
                            Aggregator = result => networkMessages = result
                        },

                        new PreProcess
                        {
                            Process = MessagePackObjectAttributeProcessor.GetMessagePackObjectTypes,
                            Aggregator = result => messagePackObjectTypes = result
                        },

                        new PreProcess
                        {
                            Process = UnionAttributeProcessor.GetUnionTypes,
                            Aggregator = result => unionTypes = result
                        },
                    }
                );

                if ((networkMessages == null || networkMessages.Count == 0)
                    && (messagePackObjectTypes == null || messagePackObjectTypes.Count == 0)
                    && (unionTypes == null || messagePackObjectTypes.Count == 0)
                )
                {
                    return;
                }

                string tempFilePath = Path.Combine("Temp", "Pretia-WeaverTempFile");
                bool tempFileFound = File.Exists(tempFilePath);

                using (var assemblyDefinition = AssemblyDefinition.ReadAssembly(assemblyPath, readerParameters))
                {
                    var mainModule = assemblyDefinition.MainModule;
                    var baseModule = GetBaseModule(mainModule);

                    mainModule.InitCommonRefs();

                    bool isBuildingPlayer = BuildPipeline.isBuildingPlayer;

                    try
                    {
                        var processes = new Task[]
                        {
                            Task.Run(() =>
                            {
                                TypeResolverProcessor.Process(mainModule, baseModule, networkMessages, isBuildingPlayer);
                            }),
                            Task.Run(() =>
                            {
                                MessagePackFormatterProcessor.Process(mainModule, baseModule, messagePackObjectTypes, unionTypes);
                            }),
                        };

                        Task.WaitAll(processes);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }

                    assemblyDefinition.Write(new WriterParameters
                    {
                        WriteSymbols = true
                    });
                }
                sw.Stop();
            }
        }

        private static ModuleDefinition GetBaseModule(ModuleDefinition mainModule)
        {
            var apiCompatibilityLevel = PlayerSettings.GetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup);
            string baseLib = null;

            switch (apiCompatibilityLevel)
            {
                case ApiCompatibilityLevel.NET_2_0:
                case ApiCompatibilityLevel.NET_2_0_Subset:
                case ApiCompatibilityLevel.NET_Standard_2_0:
                    baseLib = "netstandard";
                    break;
                default:
                    baseLib = "mscorlib";
                    break;
            }

            return mainModule.AssemblyResolver.Resolve(AssemblyNameReference.Parse(baseLib)).MainModule;
        }

    }
}