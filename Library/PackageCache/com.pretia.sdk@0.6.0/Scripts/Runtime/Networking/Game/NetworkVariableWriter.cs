using System;
using System.Collections;
using System.Numerics;
using MessagePack;
using MessagePack.Formatters;
using UnityEngine;

namespace PretiaArCloud.Networking
{
    public ref struct NetworkVariableWriter
    {
        int _fieldCounter;
        ulong _dirtyMask;

        internal ulong DirtyMask
        {
            get => _dirtyMask;
        }

        MessagePackWriter _writer;
        MessagePackSerializerOptions _options;

        internal NetworkVariableWriter(ref MessagePackWriter writer, MessagePackSerializerOptions options)
        {
            _fieldCounter = 0;
            _dirtyMask = 0;
            _writer = writer;
            _options = options;
        }
        
        public void Write<T>(NetworkVariable<T> networkVariable)
        {
            var formatter = _options.Resolver.GetFormatterWithVerify<T>() as INetworkVarMessagePackFormatter<T>;
            ulong dirtyMask = 0;
            if (networkVariable.ForceSend)
            {
                dirtyMask = formatter.FieldCount == 64 ? ulong.MaxValue : ((1UL << formatter.FieldCount)) - 1;
            }
            else
            {
                dirtyMask = formatter.GetDirtyMask(networkVariable.Before, networkVariable.Value, _options);
            }

            if (dirtyMask != 0)
            {
                formatter.SerializeNetworkVar(ref _writer, networkVariable.Value, _options, dirtyMask);
                _writer.Flush();

                _dirtyMask |= dirtyMask << _fieldCounter;

                networkVariable.ApplyChanges();
            }

            _fieldCounter += formatter.FieldCount;
        }
    }
}