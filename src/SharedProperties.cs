using System;
using System.Collections.Generic;
using UnityEngine;

namespace DCFApixels.Internal
{
    public abstract class SharedPropertiesBase<TKey, TValue> : MonoBehaviour
    {
        #region static
        protected const int ADD_SPACING = 8;

        protected static readonly Dictionary<TKey, int> _ids = new Dictionary<TKey, int>();
        protected static readonly List<TKey> _keys = new List<TKey>();

        protected static void AddKey(TKey key)
        {
            if (!_ids.ContainsKey(key))
            {
                _ids.Add(key, _keys.Count);
                _keys.Add(key);
            }
        }

        protected static bool ContainsId(int id) => id >= 0 && id < _keys.Count;
        protected static bool ContainsKey(TKey statName) => _ids.ContainsKey(statName);

        public static int GetId(TKey key)
        {
            if (!_ids.TryGetValue(key, out int id))
            {
                id = _keys.Count;
                _ids.Add(key, id);
                _keys.Add(key);
            }
            return id;
        }
        protected static TKey GetKey(int id) => _keys[id];

        protected static int CurrentCount => _ids.Count;
        #endregion

        [SerializeField]
        protected KeyValuePair[] _pairs;

        protected void Awake()
        {
            if (_isInit == false)
                Init();
        }

        private bool _isInit = false;
        protected bool IsInit => _isInit;
        protected void Init()
        {
            _isInit = true;
            for (int i = 0; i < _pairs.Length; i++)
            {
                AddKey(_pairs[i].key);
            }

            KeyValuePair[] newPairs = new KeyValuePair[CurrentCount];
            for (int i = 0; i < _pairs.Length; i++)
            {
                int id = GetId(_pairs[i].key);
                newPairs[id] = _pairs[i];
            }

            for (int i = 0; i < newPairs.Length; i++)
            {
                newPairs[i].key = GetKey(i);
            }

            _pairs = newPairs;
        }

        public bool ContainsID(int id)
        {
            return id >= 0 && id < _pairs.Length;
        }


        [Serializable]
        public struct KeyValuePair
        {
            public TKey key;
            public TValue value;
        }
    }
}
namespace DCFApixels
{
    using DCFApixels.Internal;

    public class SharedClassProperties<TKey, TValue> : SharedPropertiesBase<TKey, TValue>
        where TValue : class, new()
    {
        public TValue Get(int id)
        {
            if (IsInit == false)
                Init();

            if (id >= _pairs.Length && id < CurrentCount)
                Resize(Mathf.Min(id + ADD_SPACING, CurrentCount));
            return _pairs[id].value;
        }

        private void Resize(int newSize)
        {
            int oldLength = _pairs.Length;
            Array.Resize(ref _pairs, newSize);
            for (int i = oldLength; i < newSize; i++)
            {
                ref KeyValuePair pair = ref _pairs[i];
                pair.key = GetKey(i);
                pair.value = new();
            }
        }
    }

    public class SharedValueProperties<TKey, TValue> : SharedPropertiesBase<TKey, TValue>
       where TValue : notnull
    {
        public ref TValue Get(int id)
        {
            if (IsInit == false)
                Init();

            if (id >= _pairs.Length && id < CurrentCount)
                Resize(Mathf.Min(id + ADD_SPACING, CurrentCount));
            return ref _pairs[id].value;
        }

        private void Resize(int newSize)
        {
            int oldLength = _pairs.Length;
            Array.Resize(ref _pairs, newSize);
            for (int i = oldLength; i < newSize; i++)
            {
                ref KeyValuePair pair = ref _pairs[i];
                pair.key = GetKey(i);
            }
        }
    }
}