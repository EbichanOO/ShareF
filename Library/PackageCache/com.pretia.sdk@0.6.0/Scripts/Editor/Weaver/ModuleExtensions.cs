using Mono.Cecil;
using UnityEditor;
using UnityEngine;

namespace PretiaArCloud.Networking.Weaver
{
    internal static class ModuleExtensions
    {
        private static MethodReference _initializeOnLoadMethod_constructor;

        private static TypeReference _runtimeInitializeOnLoadMethod_typeRef;
        private static MethodReference _runtimeInitializeOnLoadMethod_constructor;

        internal static void InitCommonRefs(this ModuleDefinition module)
        {
            _runtimeInitializeOnLoadMethod_typeRef = module.ImportReference(typeof(RuntimeInitializeLoadType));

            System.Reflection.ConstructorInfo runtimeInitializeOnLoadMethod_constructor = typeof(RuntimeInitializeOnLoadMethodAttribute).GetConstructor(new [] { typeof(RuntimeInitializeLoadType)});
            _runtimeInitializeOnLoadMethod_constructor = module.ImportReference(runtimeInitializeOnLoadMethod_constructor);

            if (!BuildPipeline.isBuildingPlayer)
            {
                System.Reflection.ConstructorInfo initializeOnLoadMethod_constructor = typeof(InitializeOnLoadMethodAttribute).GetConstructor(System.Type.EmptyTypes);
                _initializeOnLoadMethod_constructor = module.ImportReference(initializeOnLoadMethod_constructor);
            }
        }

        internal static TypeDefinition GeneratedClass(this ModuleDefinition module)
        {
            TypeDefinition type = module.GetType("PretiaArCloud.Networking", "GeneratedNetworkCode");

            if (type != null)
                return type;

            type = new TypeDefinition("PretiaArCloud.Networking", "GeneratedNetworkCode",
                        TypeAttributes.BeforeFieldInit | TypeAttributes.Class | TypeAttributes.AnsiClass | TypeAttributes.Public | TypeAttributes.AutoClass | TypeAttributes.Abstract | TypeAttributes.Sealed,
                        module.ImportReference(typeof(object)));
            module.Types.Add(type);
            return type;
        }

        internal static void AddRuntimeInitializeOnLoadMethodAttribute(this MethodDefinition methodDef, RuntimeInitializeLoadType runtimeInitializeLoadType)
        {
            var customAttributeRef = new CustomAttribute(_runtimeInitializeOnLoadMethod_constructor);
            customAttributeRef.ConstructorArguments.Add(new CustomAttributeArgument(_runtimeInitializeOnLoadMethod_typeRef, runtimeInitializeLoadType));
            methodDef.CustomAttributes.Add(customAttributeRef);
        }

        internal static void AddInitializeOnLoadMethoAttribute(this MethodDefinition methodDef)
        {
            var customAttributeRef = new CustomAttribute(_initializeOnLoadMethod_constructor);
            methodDef.CustomAttributes.Add(customAttributeRef);
        }
    }
}