namespace PretiaArCloud.Networking
{
    public interface INetworkVariable<T> 
    {
        T Value { get; set; }
    }
}