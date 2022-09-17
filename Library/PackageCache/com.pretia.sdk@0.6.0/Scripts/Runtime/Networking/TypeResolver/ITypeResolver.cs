using System;

namespace PretiaArCloud.Networking
{
    public interface ITypeResolver
    {
        ushort Resolve(OpType opType, Type type);
    }
}