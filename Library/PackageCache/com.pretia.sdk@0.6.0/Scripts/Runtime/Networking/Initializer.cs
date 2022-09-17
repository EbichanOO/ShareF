using UnityEngine;

namespace PretiaArCloud.Networking
{
    internal static class Initializer
    {
        private const int STARTING_INDEX = 32768;

        [RuntimeInitializeOnLoadMethod]
        private static void InitResolvers()
        {
            // Reserve Id from 32768-65536 for Pretia's own network message type
            TypeResolver.Add(typeof(DeleteExistInSceneObjectsMsg), STARTING_INDEX);
            TypeResolver.Add(typeof(InstantiateMsg), STARTING_INDEX + 1);
            TypeResolver.Add(typeof(NetworkAnimatorSnapshotMsg), STARTING_INDEX + 2);
            TypeResolver.Add(typeof(NetworkAnimatorSyncMsg), STARTING_INDEX + 3);
            TypeResolver.Add(typeof(NetworkDestroyMsg), STARTING_INDEX + 4);
            TypeResolver.Add(typeof(NetworkRigidbodySyncMsg), STARTING_INDEX + 5);
            TypeResolver.Add(typeof(NetworkTransformSyncMsg), STARTING_INDEX + 6);
            TypeResolver.Add(typeof(NotifyConnectionMsg), STARTING_INDEX + 7);
            TypeResolver.Add(typeof(PlayerJoinedMsg), STARTING_INDEX + 8);
            TypeResolver.Add(typeof(PlayerListSnapshotMsg), STARTING_INDEX + 9);
        }
    }
}