using MessagePack;

namespace PretiaArCloud.Networking
{
    [NetworkMessage]
    [MessagePackObject]
    public class NotifyConnectionMsg
    {
        [Key(0)]
        public string DisplayName;
    }
}