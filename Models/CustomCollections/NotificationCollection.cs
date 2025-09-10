using MunicipalServicesApp.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MunicipalServicesApp.Models.CustomCollections
{
    [JsonConverter(typeof(NotificationCollectionConverter))]
    public class NotificationCollection : IEnumerable<Notification>
    {
        private NotificationNode _head;
        private NotificationNode _tail;
        private int _count;

        private class NotificationNode
        {
            public Notification Data { get; }
            public NotificationNode Next { get; set; }
            public NotificationNode Previous { get; set; }

            public NotificationNode(Notification data)
            {
                Data = data ?? throw new ArgumentNullException(nameof(data));
            }
        }

        public int Count => _count;

        public void Add(Notification notification)
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));

            var newNode = new NotificationNode(notification);

            if (_head == null)
            {
                _head = newNode;
                _tail = newNode;
            }
            else
            {
                _tail.Next = newNode;
                newNode.Previous = _tail;
                _tail = newNode;
            }

            _count++;
        }

        public Notification FindById(string id)
        {
            var current = _head;
            while (current != null)
            {
                if (current.Data.Id == id)
                    return current.Data;
                current = current.Next;
            }
            return null;
        }

        public NotificationCollection GetUnreadNotifications()
        {
            var result = new NotificationCollection();
            foreach (var n in this)
            {
                if (!n.IsRead)
                    result.Add(n);
            }
            return result;
        }

        public Notification GetAt(int index)
        {
            if (index < 0 || index >= _count)
                throw new IndexOutOfRangeException();

            var current = _head;
            for (int i = 0; i < index; i++)
                current = current.Next;

            return current.Data;
        }

        public List<Notification> ToList()
        {
            var list = new List<Notification>();
            foreach (var n in this)
                list.Add(n);
            return list;
        }

        public void FromList(List<Notification> list)
        {
            _head = null;
            _tail = null;
            _count = 0;

            if (list != null)
            {
                foreach (var item in list)
                    Add(item);
            }
        }

        public IEnumerator<Notification> GetEnumerator()
        {
            var current = _head;
            while (current != null)
            {
                yield return current.Data;
                current = current.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class NotificationCollectionConverter : JsonConverter<NotificationCollection>
    {
        public override void WriteJson(JsonWriter writer, NotificationCollection value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value?.ToList());
        }

        public override NotificationCollection ReadJson(JsonReader reader, Type objectType, NotificationCollection existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var list = serializer.Deserialize<List<Notification>>(reader);
            var collection = new NotificationCollection();
            collection.FromList(list);
            return collection;
        }
    }
}
