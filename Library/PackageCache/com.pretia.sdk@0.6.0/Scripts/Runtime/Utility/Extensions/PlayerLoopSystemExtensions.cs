using System;
using UnityEngine.LowLevel;

namespace PretiaArCloud
{
    internal static class PlayerLoopSystemExtensions
    {
        internal static PlayerLoopSystem FindSubSystem<T>(this PlayerLoopSystem def)
        {
            if (def.type == typeof(T))
            {
                return def;
            }
            if (def.subSystemList != null)
            {
                foreach (var s in def.subSystemList)
                {
                    var system = s.FindSubSystem<T>();
                    if (system.type == typeof(T))
                    {
                        return system;
                    }
                }
            }
            return default(PlayerLoopSystem);
        }

        internal static void AddSubSystemAfter<T>(ref this PlayerLoopSystem parent, PlayerLoopSystem subSystem)
        {
            int index = Array.FindIndex(parent.subSystemList, s => s.type == typeof(T));
            PlayerLoopSystem[] subSystemListCopy = new PlayerLoopSystem[parent.subSystemList.Length + 1];

            if (index > 0) 
                Array.Copy(parent.subSystemList, subSystemListCopy, index+1);

            subSystemListCopy[index+1] = subSystem;

            if (index < parent.subSystemList.Length)
                Array.Copy(parent.subSystemList, index+1, subSystemListCopy, index+2, parent.subSystemList.Length - index - 1);

            parent.subSystemList = subSystemListCopy;
        }

        internal static void ReplaceSubSystem<T>(ref this PlayerLoopSystem parent, PlayerLoopSystem subSystem)
        {
            int index = Array.FindIndex(parent.subSystemList, s => s.type == typeof(T));
            parent.subSystemList[index] = subSystem;
        }
    }
}