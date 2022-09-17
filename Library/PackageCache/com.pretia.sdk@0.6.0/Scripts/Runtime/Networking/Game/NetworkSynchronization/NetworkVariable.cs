namespace PretiaArCloud.Networking
{
    public class NetworkVariable<T> : INetworkVariable<T>
    {
        internal T Before { get; private set; }

        public T Value { get; set; }

        public bool ForceSend { get; private set; }

        public NetworkVariable(bool forceSend = false)
        {
            Before = default(T);
            Value = default(T);
            ForceSend = forceSend;
        }

        public NetworkVariable(T initialValue, bool forceSend = false)
        {
            Before = initialValue;
            Value = initialValue;
            ForceSend = forceSend;
        }

        public void ApplyChanges()
        {
            Before = Value;
        }
    }
}