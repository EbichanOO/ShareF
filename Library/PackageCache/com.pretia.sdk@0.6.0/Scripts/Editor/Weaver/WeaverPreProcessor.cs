using System;
using System.Collections.Generic;
using Mono.Cecil;

namespace PretiaArCloud.Networking.Weaver
{
    internal struct PreProcess
    {
        public Func<AssemblyDefinition, IEnumerable<TypeDefinition>> Process;
        public Action<List<TypeDefinition>> Aggregator;
    }

    internal class WeaverPreProcessor
    {
        internal static void Execute(
            string assemblyPath,
            ReaderParameters readerParameters,
            params PreProcess[] preProcesses
        )
        {
            List<TypeDefinition>[] results = new List<TypeDefinition>[preProcesses.Length];
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = new List<TypeDefinition>();
            }
            
            using (var asmDef = AssemblyDefinition.ReadAssembly(assemblyPath, readerParameters))
            {
                for (int i = 0; i < preProcesses.Length; i++)
                {
                    ExecutePreProcesses(preProcesses[i].Process, asmDef, results[i]);
                }
            }

            for (int i = 0; i < preProcesses.Length; i++)
            {
                preProcesses[i].Aggregator(results[i]);
            }
        }

        private static void ExecutePreProcesses(Func<AssemblyDefinition, IEnumerable<TypeDefinition>> preProcess, AssemblyDefinition assemblyDefinition, List<TypeDefinition> resultAggregate)
        {
            var result = preProcess(assemblyDefinition);
            lock (resultAggregate)
            {
                resultAggregate.AddRange(result);
            }
        }
    }
}
