using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;

namespace PretiaArCloud.Networking
{
    using Resolvers;
    public class MsgPackPretiaResolver : IFormatterResolver
    {
        private static IFormatterResolver[] Resolvers = default;

        internal MsgPackPretiaResolver(IFormatterResolver appResolver)
        {
            Resolvers = new IFormatterResolver[]
            {
                AttributeFormatterResolver.Instance, // Try use [MessagePackFormatter]
                PretiaGeneratedResolver.Instance,
                appResolver,

                BuiltinResolver.Instance, // Try Builtin
                DynamicGenericResolver.Instance,

#if UNITY_2018_3_OR_NEWER
                MessagePack.Unity.UnityResolver.Instance,
#endif

            };
        }

        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            return Cache<T>.Formatter;
        }

        private static class Cache<T>
        {
            public static IMessagePackFormatter<T> Formatter;

            static Cache()
            {
                foreach (var resolver in Resolvers)
                {
                    var f = resolver.GetFormatter<T>();
                    if (f != null)
                    {
                        Formatter = f;
                        return;
                    }
                }
            }
        }
    }
}