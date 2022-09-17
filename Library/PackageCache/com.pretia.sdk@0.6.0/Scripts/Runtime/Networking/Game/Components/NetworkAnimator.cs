using System.Collections.Generic;
using MessagePack;
using UnityEngine;

namespace PretiaArCloud.Networking
{
    public class NetworkAnimator : NetworkBehaviour
    {
        public Animator Animator;

        [HideInInspector]
        public bool[] SynchronizedParameters;

        private Dictionary<int, bool> _triggerStore = new Dictionary<int, bool>();

        private void Awake()
        {
            for (int i = 0; i < Animator.parameterCount; i++)
            {
                if (Animator.parameters[i].type == AnimatorControllerParameterType.Trigger)
                {
                    _triggerStore.Add(Animator.parameters[i].nameHash, false);
                }
            }
        }

        public void SetTrigger(string name)
        {
            SetTrigger(Animator.StringToHash(name));
        }

        public void SetTrigger(int nameHash)
        {
            if (_triggerStore.ContainsKey(nameHash))
            {
                _triggerStore[nameHash] = true;
                Animator.SetTrigger(nameHash);
            }
        }

        public override void Serialize(ref MessagePackWriter writer, MessagePackSerializerOptions options)
        {
            // always send
            _dirtyMask = 1;

            for (int i = 0; i < Animator.parameterCount; i++)
            {
                if (!SynchronizedParameters[i]) continue;

                var parameter = Animator.parameters[i];
                switch (parameter.type)
                {
                    case AnimatorControllerParameterType.Float:
                        writer.Write(Animator.GetFloat(parameter.nameHash));
                        break;
                    case AnimatorControllerParameterType.Int:
                        writer.WriteInt32(Animator.GetInteger(parameter.nameHash));
                        break;
                    case AnimatorControllerParameterType.Bool:
                        writer.Write(Animator.GetBool(parameter.nameHash));
                        break;
                }
            }

            if (_triggerStore.Count > 0)
            {
                int triggerMask = 0;
                int i = 0;
                foreach (var trigger in _triggerStore)
                {
                    if (trigger.Value)
                    {
                        _triggerStore[trigger.Key] = false;
                        triggerMask |= 1 << i;
                    }

                    i++;
                }
                writer.WriteInt32(triggerMask);
            }
        }

        public override void Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            for (int i = 0; i < Animator.parameterCount; i++)
            {
                if (!SynchronizedParameters[i]) continue;

                var parameter = Animator.parameters[i];
                switch (parameter.type)
                {
                    case AnimatorControllerParameterType.Float:
                        Animator.SetFloat(parameter.nameHash, reader.ReadSingle());
                        break;
                    case AnimatorControllerParameterType.Int:
                        Animator.SetInteger(parameter.nameHash, reader.ReadInt32());
                        break;
                    case AnimatorControllerParameterType.Bool:
                        Animator.SetBool(parameter.nameHash, reader.ReadBoolean());
                        break;
                }
            }

            if (_triggerStore.Count > 0)
            {
                int triggerMask = reader.ReadInt32();
                int i = 0;
                foreach (var trigger in _triggerStore)
                {
                    int currentMask = 1 << i;
                    if ((triggerMask & currentMask) != 0)
                    {
                        Animator.SetTrigger(trigger.Key);
                    }
                    i++;
                }
            }
        }
    }
}