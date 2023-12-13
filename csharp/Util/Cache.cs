using System.Collections.Generic;

namespace AdventOfCode.Util
{
    public class Cache<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _dict = new();
        
        public int Hits { get; private set; }
        public int Misses { get; private set; }

        public TValue this[TKey key]
        {
            set => _dict[key] = value;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (_dict.TryGetValue(key, out value))
            {
                Hits++;
                return true;
            }

            Misses++;
            return false;
        }
    }
}
