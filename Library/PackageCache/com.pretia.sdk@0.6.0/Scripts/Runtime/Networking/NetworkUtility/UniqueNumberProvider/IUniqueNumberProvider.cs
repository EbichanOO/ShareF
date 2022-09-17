using System;

namespace PretiaArCloud.Networking
{
    public interface IUniqueNumberProvider<T>
    {
        T Rent();
        void Return(T number);
    }
}
