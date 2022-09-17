using System;
using UnityEngine;
using UnityEngine.LowLevel;

namespace PretiaArCloud.Networking
{
    internal static class NetworkLifecycle
    {
        internal static event Action SyncUpdate;

        [RuntimeInitializeOnLoadMethod]
        private static void AppStart()
        {
            var playerLoop = PlayerLoop.GetCurrentPlayerLoop();
            var preUpdate = playerLoop.FindSubSystem<UnityEngine.PlayerLoop.FixedUpdate>();

            preUpdate.AddSubSystemAfter<UnityEngine.PlayerLoop.FixedUpdate.ScriptRunDelayedFixedFrameRate>(NetworkSyncUpdate.CreateLoopSystem());
            playerLoop.ReplaceSubSystem<UnityEngine.PlayerLoop.FixedUpdate>(preUpdate);

            PlayerLoop.SetPlayerLoop(playerLoop);
        }

        private static void _SyncUpdate()
        {
            SyncUpdate?.Invoke();
        }

        internal struct NetworkSyncUpdate
        {
            internal static PlayerLoopSystem CreateLoopSystem() =>
                new PlayerLoopSystem
                    {
                        type = typeof(NetworkSyncUpdate),
                        updateDelegate = _SyncUpdate,
                    };
        }
    }
}