using MessagePack;
using MessagePack.Formatters;

namespace PretiaArCloud.Networking
{
    public ref struct NetworkVariableReader
    {
        int _fieldCounter;
        ulong _dirtyMask;
        MessagePackReader _reader;
        MessagePackSerializerOptions _options;

        public long Consumed => _reader.Consumed;

        internal NetworkVariableReader(ref MessagePackReader reader, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            _fieldCounter = 0;
            _dirtyMask = dirtyMask;
            _reader = reader;
            _options = options;
        }

        public void Read<T>(NetworkVariable<T> networkVariable)
        {
            var formatter = _options.Resolver.GetFormatterWithVerify<T>() as INetworkVarMessagePackFormatter<T>;
            var temp = networkVariable.Value;
            formatter.DeserializeNetworkVar(ref _reader, _options, ref temp, _dirtyMask >> _fieldCounter);
            networkVariable.Value = temp;
            _fieldCounter += formatter.FieldCount;
        }
    }
}