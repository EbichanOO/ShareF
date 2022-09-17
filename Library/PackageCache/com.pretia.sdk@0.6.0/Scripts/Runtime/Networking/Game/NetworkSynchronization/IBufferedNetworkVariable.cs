namespace PretiaArCloud.Networking
{
    public interface IBufferedNetworkVariable<T> : INetworkVariable<T>
    {
        T this[int tick] { get; }
        bool TryGetValue(int tick, out T value);
    }
}