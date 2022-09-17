using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using PretiaArCloudEditor;
using UnityEditor;
using UnityEngine;

namespace PretiaArCloud.Networking.Weaver
{
    internal class TypeResolverProcessor
    {
        internal static void Process(ModuleDefinition mainModule,
                                     ModuleDefinition baseModule,
                                     List<TypeDefinition> networkMessages,
                                     bool isBuildingPlayer)
        {
            var initialize_methodDef = mainModule.GeneratedClass().AddMethod("__GENERATED__InitTypeResolvers", MethodAttributes.Static | MethodAttributes.Public);
            if (isBuildingPlayer)
            {
                initialize_methodDef.AddRuntimeInitializeOnLoadMethodAttribute(RuntimeInitializeLoadType.BeforeSceneLoad);
            }
            else
            {
                initialize_methodDef.AddInitializeOnLoadMethoAttribute();
            }

            var instructions = initialize_methodDef.Body.Instructions;
            var iLProcessor = initialize_methodDef.Body.GetILProcessor();

            var pretiaModule = mainModule.AssemblyResolver.Resolve(AssemblyNameReference.Parse("PretiaSDK.Runtime")).MainModule;

            var typeResolver_typeDef = pretiaModule.Types.FirstOrDefault(t => t.Name == "TypeResolver");
            var add_methodRef = mainModule.ImportReference(typeResolver_typeDef.Methods.FirstOrDefault(m => m.Name == "Add"));
            var getTypeFromHandle_methodRef = mainModule.ImportReference(
                    baseModule.Types
                    .FirstOrDefault(t => t.Name == "Type")
                    .Methods
                    .FirstOrDefault(m => m.Name == "GetTypeFromHandle")
                );

            for (int i = 0; i < networkMessages.Count; i++)
            {
                TypeReference networkMessageType = mainModule.GetType(networkMessages[i].FullName);
                if (networkMessageType == null)
                {
                    networkMessageType = mainModule.ImportReference(networkMessages[i]);
                }

                string typeName = $"{mainModule.Name}+{networkMessageType.FullName}";
                int hash = typeName.GetStableHashCode() & 0x7fff; //id range: 0-32767, 32768-65535 is reserved for internal use

                iLProcessor.Emit(OpCodes.Ldtoken, networkMessageType);
                iLProcessor.Emit(OpCodes.Call, getTypeFromHandle_methodRef);
                iLProcessor.Append(iLProcessor.CreateLoadIntegerInstruction(hash));
                iLProcessor.Emit(OpCodes.Call, add_methodRef);
            }

            iLProcessor.Emit(OpCodes.Ret);
        }
    }
}
