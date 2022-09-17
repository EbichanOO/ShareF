using MessagePack.Formatters;

namespace PretiaArCloud.Networking
{
    public static class Formatter<T>
    {
        public static IMessagePackFormatter<T> Get { internal get; set; }
    }
}