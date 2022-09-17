using System;
using System.Threading;

namespace PretiaArCloud.Networking
{
    internal class UnityMainThreadExecutor : IMainThreadExecutor
    {
        private SynchronizationContext _mainThread;

        public UnityMainThreadExecutor(SynchronizationContext mainThread)
        {
            _mainThread = mainThread;
        }

        public void Execute(Action action)
        {
            _mainThread.Post(delegate { action(); }, null);
        }
    }
}