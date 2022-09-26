using DCFApixels.Internal;
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
        private void Init()
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

        public ref TValue Get(int statId)
        {
            if (_isInit == false)
                Init();

            if (statId >= _pairs.Length && statId < CurrentCount)
                Resize(Mathf.Min(statId + ADD_SPACING, CurrentCount));
            return ref _pairs[statId].value;
        }

        protected abstract void Resize(int newSize);

        public bool ContainsID(int statId)
        {
            return statId >= 0 && statId < _pairs.Length;
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
    public class SharedClassProperties<TKey, TValue> : SharedPropertiesBase<TKey, TValue>
        where TValue : class, new()
    {
        protected override void Resize(int newSize)
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
        protected override void Resize(int newSize)
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