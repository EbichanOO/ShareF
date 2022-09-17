using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace PretiaArCloud.Networking.Weaver
{
    internal class MessagePackObjectAttributeProcessor
    {
        internal static IEnumerable<TypeDefinition> GetMessagePackObjectTypes(AssemblyDefinition source)
        {
            return from type in source.MainModule.GetAllTypes()
                   from attr in type.CustomAttributes
                   where attr.AttributeType.Name == "MessagePackObjectAttribute"
                   select type;
        }
    }
}
