using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MunicipalServicesApp.Models.CustomCollections
{
    [JsonConverter(typeof(CustomStackConverter<>))]
    public class CustomStack<T> : IEnumerable<T>
    {
        private StackNode _top;
        private int _count;

        private class StackNode
        {
            public T Data { get; set; }
            public StackNode Next { get; set; }

            public StackNode(T data)
            {
                Data = data;
            }
        }

        public int Count => _count;

        public void Push(T item)
        {
            var newNode = new StackNode(item)
            {
                Next = _top
            };
            _top = newNode;
            _count++;
        }

        public T Pop()
        {
            if (_top == null)
                throw new InvalidOperationException("Stack is empty");

            var data = _top.Data;
            _top = _top.Next;
            _count--;
            return data;
        }

        public T Peek()
        {
            if (_top == null)
                throw new InvalidOperationException("Stack is empty");

            return _top.Data;
        }

        public bool IsEmpty => _top == null;

        // Method to convert to list for serialization
        public List<T> ToList()
        {
            var list = new List<T>();
            var current = _top;
            while (current != null)
            {
                list.Add(current.Data);
                current = current.Next;
            }
            list.Reverse(); // Stack is LIFO, so reverse to maintain order
            return list;
        }

        // Method to initialize from list
        public void FromList(List<T> list)
        {
            _top = null;
            _count = 0;

            if (list != null)
            {
                // Push in reverse order to maintain stack order
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    Push(list[i]);
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            var current = _top;
            while (current != null)
            {
                yield return current.Data;
                current = current.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class CustomStackConverter<T> : JsonConverter<CustomStack<T>>
    {
        public override void WriteJson(JsonWriter writer, CustomStack<T> value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value?.ToList());
        }

        public override CustomStack<T> ReadJson(JsonReader reader, Type objectType, CustomStack<T> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var list = serializer.Deserialize<List<T>>(reader);
            var stack = new CustomStack<T>();
            stack.FromList(list);
            return stack;
        }
    }
}