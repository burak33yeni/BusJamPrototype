using System.Collections;
using System.Collections.Generic;

namespace Core.Tools
{
    public class Map<TKey, TValue> : IDictionary<TKey, TValue>,
        IReadOnlyDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _forward = new();
        private readonly Dictionary<TValue, TKey> _reverse = new();

        public int Count => _forward.Count;
        public bool IsReadOnly => false;
        public ICollection<TKey> Keys => _forward.Keys;
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => _forward.Keys;
        public ICollection<TValue> Values => _reverse.Keys;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => _reverse.Keys;

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (!ContainsKey(key))
            {
                value = default;
                return false;
            }

            value = this[key];
            return true;
        }

        public TValue this[TKey index]
        {
            get => _forward[index];
            set => _forward[index] = value;
        }

        public void Add(TKey key, TValue value)
        {
            _forward.Add(key, value);
            _reverse.Add(value, key);
        }

        private bool Remove(TKey key)
        {
            if (!ContainsKey(key)) return false;
            TValue value = _forward[key];
            _forward.Remove(key);
            _reverse.Remove(value);
            return true;
        }

        public bool ContainsKey(TKey key)
        {
            return _forward.ContainsKey(key);
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            return Remove(key);
        }

        public bool ContainsValue(TValue value)
        {
            return _reverse.ContainsKey(value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _forward.Clear();
            _reverse.Clear();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _forward.GetEnumerator();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}