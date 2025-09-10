using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MunicipalServicesApp.Models.CustomCollections
{
    [JsonConverter(typeof(CustomDictionaryConverter<,>))]
    public class CustomDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private const int InitialCapacity = 16;
        private KeyValueNode[] _buckets;
        private int _count;

        private class KeyValueNode
        {
            public TKey Key { get; set; }
            public TValue Value { get; set; }
            public KeyValueNode Next { get; set; }
        }

        public CustomDictionary()
        {
            _buckets = new KeyValueNode[InitialCapacity];
        }

        public int Count => _count;

        public void Add(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var bucketIndex = GetBucketIndex(key);
            var currentNode = _buckets[bucketIndex];

            // Check if key already exists
            while (currentNode != null)
            {
                if (currentNode.Key.Equals(key))
                    throw new ArgumentException("Key already exists");
                currentNode = currentNode.Next;
            }

            // Add new node
            var newNode = new KeyValueNode { Key = key, Value = value };
            newNode.Next = _buckets[bucketIndex];
            _buckets[bucketIndex] = newNode;
            _count++;

            // Resize if needed
            if ((double)_count / _buckets.Length > 0.75)
            {
                Resize();
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null)
            {
                value = default;
                return false;
            }

            var bucketIndex = GetBucketIndex(key);
            var currentNode = _buckets[bucketIndex];

            while (currentNode != null)
            {
                if (currentNode.Key.Equals(key))
                {
                    value = currentNode.Value;
                    return true;
                }
                currentNode = currentNode.Next;
            }

            value = default;
            return false;
        }

        public bool ContainsKey(TKey key)
        {
            return TryGetValue(key, out _);
        }

        private int GetBucketIndex(TKey key)
        {
            return Math.Abs(key.GetHashCode()) % _buckets.Length;
        }

        private void Resize()
        {
            var newBuckets = new KeyValueNode[_buckets.Length * 2];

            foreach (var oldBucket in _buckets)
            {
                var currentNode = oldBucket;
                while (currentNode != null)
                {
                    var nextNode = currentNode.Next;
                    var newBucketIndex = Math.Abs(currentNode.Key.GetHashCode()) % newBuckets.Length;

                    currentNode.Next = newBuckets[newBucketIndex];
                    newBuckets[newBucketIndex] = currentNode;

                    currentNode = nextNode;
                }
            }

            _buckets = newBuckets;
        }

        // Method to convert to dictionary for serialization
        public Dictionary<TKey, TValue> ToDictionary()
        {
            var dict = new Dictionary<TKey, TValue>();
            foreach (var bucket in _buckets)
            {
                var currentNode = bucket;
                while (currentNode != null)
                {
                    dict[currentNode.Key] = currentNode.Value;
                    currentNode = currentNode.Next;
                }
            }
            return dict;
        }

        // Method to initialize from dictionary
        public void FromDictionary(Dictionary<TKey, TValue> dict)
        {
            _buckets = new KeyValueNode[InitialCapacity];
            _count = 0;

            if (dict != null)
            {
                foreach (var kvp in dict)
                {
                    Add(kvp.Key, kvp.Value);
                }
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var bucket in _buckets)
            {
                var currentNode = bucket;
                while (currentNode != null)
                {
                    yield return new KeyValuePair<TKey, TValue>(currentNode.Key, currentNode.Value);
                    currentNode = currentNode.Next;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class CustomDictionaryConverter<TKey, TValue> : JsonConverter<CustomDictionary<TKey, TValue>>
    {
        public override void WriteJson(JsonWriter writer, CustomDictionary<TKey, TValue> value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value?.ToDictionary());
        }

        public override CustomDictionary<TKey, TValue> ReadJson(JsonReader reader, Type objectType, CustomDictionary<TKey, TValue> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var dict = serializer.Deserialize<Dictionary<TKey, TValue>>(reader);
            var customDict = new CustomDictionary<TKey, TValue>();
            customDict.FromDictionary(dict);
            return customDict;
        }
    }
}