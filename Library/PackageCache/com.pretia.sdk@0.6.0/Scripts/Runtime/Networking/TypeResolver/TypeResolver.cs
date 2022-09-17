using System;
using System.Collections.Generic;
using UnityEngine;

namespace PretiaArCloud.Networking
{
    public static class TypeResolver
    {
        private static HashSet<ushort> _usedIds = new HashSet<ushort>();
        private static Dictionary<Type, ushort> _lookup = new Dictionary<Type, ushort>();

        public static bool NoCollisions = true;

        public static void Add(Type type, ushort id)
        {
            if (Application.isEditor)
            {
                if (_usedIds.Contains(id))
                {
                    NoCollisions = false;
                    string assemblyName = type.Assembly.GetName().Name;
                    Debug.LogError($"Hash id of type {assemblyName}+{type} collides with an existing type. Please change the name to avoid collision");
                }
                else
                {
                    _usedIds.Add(id);
                    _lookup.Add(type, id);
                }
            }
            else
            {
                _lookup.Add(type, id);
            }
        }

        public static ushort Get(Type type)
        {
            return _lookup[type];
        }

        public static ushort Get<T>()
        {
            return Get(typeof(T));
        }
    }
}