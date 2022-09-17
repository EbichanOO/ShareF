namespace PretiaArCloud
{
    public interface ISubsystem
    {
        void Initialize();
        void Resume();
        void Reset();
        void Pause();
        void Destroy();
    }
}