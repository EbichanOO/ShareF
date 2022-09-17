using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace PretiaArCloud.Networking.Weaver
{
    internal class UnionAttributeProcessor
    {
        internal static IEnumerable<TypeDefinition> GetUnionTypes(AssemblyDefinition source)
        {
            return from type in source.MainModule.GetAllTypes()
                   where type.CustomAttributes.Any(a => a.AttributeType.Name == "UnionAttribute")
                   select type;
        }
    }
}
