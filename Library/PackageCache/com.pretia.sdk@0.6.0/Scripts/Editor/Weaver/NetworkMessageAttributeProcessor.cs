using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace PretiaArCloud.Networking.Weaver
{
    internal class NetworkMessageAttributeProcessor
    {
        internal static IEnumerable<TypeDefinition> GetNetworkMessages(AssemblyDefinition source)
        {
            var networkMessages =
                from type in source.MainModule.GetAllTypes()
                from attr in type.CustomAttributes
                where attr.AttributeType.Name == nameof(NetworkMessageAttribute)
                select type;

            return networkMessages;
        }
    }
}
