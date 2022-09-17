namespace PretiaArCloud.Networking
{
    public class TickManager
    {
        private int _tick;
        internal ushort Tick { get => (ushort)_tick; } 

        internal void Increment() {
            _tick = (_tick+1) % ushort.MaxValue;
        }
    }
}
