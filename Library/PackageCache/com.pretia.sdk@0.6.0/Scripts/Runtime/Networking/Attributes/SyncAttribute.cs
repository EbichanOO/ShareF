using System;

namespace PretiaArCloud.Networking
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class SyncAttribute : Attribute { }
}