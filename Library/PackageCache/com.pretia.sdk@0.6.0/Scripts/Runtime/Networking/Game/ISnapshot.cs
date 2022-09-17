using System;

namespace PretiaArCloud.Networking
{
    public interface ISnapshot
    {
        void EnqueueSnapshot(HostToPlayerMessageHandler hostMsg);
        void RegisterSnapshotCallback(HostToPlayerMessageHandler hostMsg);
    }
}