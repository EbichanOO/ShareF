using Mono.Cecil;

namespace PretiaArCloud.Networking.Weaver
{
    internal static class TypeExtensions
    {
        internal static MethodDefinition AddMethod(this TypeDefinition typeDefinition, string name, MethodAttributes attributes, TypeReference typeReference)
        {
            var method = new MethodDefinition(name, attributes, typeReference);
            typeDefinition.Methods.Add(method);
            return method;
        }

        internal static MethodDefinition AddMethod(this TypeDefinition typeDefinition, string name, MethodAttributes attributes) =>
            AddMethod(typeDefinition, name, attributes, typeDefinition.Module.ImportReference(typeof(void)));
    }
}