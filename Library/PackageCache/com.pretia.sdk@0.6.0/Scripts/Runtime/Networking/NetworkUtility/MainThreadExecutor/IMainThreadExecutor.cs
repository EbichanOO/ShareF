namespace PretiaArCloud.Networking
{
    public interface IMainThreadExecutor
    {
        void Execute(System.Action action);
    }
}