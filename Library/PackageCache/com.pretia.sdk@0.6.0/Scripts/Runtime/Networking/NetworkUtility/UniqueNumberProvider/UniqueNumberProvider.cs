using System;
using System.Collections.Generic;

namespace PretiaArCloud.Networking
{
    public class UniqueUIntProvider : IUniqueNumberProvider<uint>
    {
        private Queue<uint> _returned = default;
        private uint _current = 1;
        private object _synch;

        public UniqueUIntProvider()
        {
            _synch = new object();
            _returned = new Queue<uint>();
        }

        public uint Rent()
        {
            lock (_synch)
            {
                if (_returned.Count > 0)
                {
                    return _returned.Dequeue();
                }
                else
                {
                    uint uniqueNumber = _current;
                    _current++;

                    return uniqueNumber;
                }
            }
        }

        public void TryUpdateCurrent(uint number)
        {
            if (_current <= number)
            {
                _current = number + 1;
            }
        }

        public void Return(uint number)
        {
            lock (_synch)
            {
                _returned.Enqueue(number);
            }
        }
    }
}
