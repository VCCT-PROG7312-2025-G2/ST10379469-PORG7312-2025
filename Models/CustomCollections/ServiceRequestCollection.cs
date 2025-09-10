using MunicipalServicesApp.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MunicipalServicesApp.Models.CustomCollections
{
    [JsonConverter(typeof(ServiceRequestCollectionConverter))]
    public class ServiceRequestCollection : IEnumerable<ServiceRequest>
    {
        private ServiceRequestNode _head;
        private ServiceRequestNode _tail;
        private int _count;

        private class ServiceRequestNode
        {
            public ServiceRequest Data { get; }
            public ServiceRequestNode Next { get; set; }
            public ServiceRequestNode Previous { get; set; }

            public ServiceRequestNode(ServiceRequest data)
            {
                Data = data ?? throw new ArgumentNullException(nameof(data));
            }
        }

        public int Count => _count;

        public void Add(ServiceRequest serviceRequest)
        {
            if (serviceRequest == null) throw new ArgumentNullException(nameof(serviceRequest));

            var newNode = new ServiceRequestNode(serviceRequest);

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

        public ServiceRequest FindById(string id)
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

        public ServiceRequestCollection GetRequestsByStatus(RequestStatus status)
        {
            var result = new ServiceRequestCollection();
            foreach (var r in this)
            {
                if (r.Status == status)
                    result.Add(r);
            }
            return result;
        }

        public ServiceRequest GetAt(int index)
        {
            if (index < 0 || index >= _count)
                throw new IndexOutOfRangeException();

            var current = _head;
            for (int i = 0; i < index; i++)
                current = current.Next;

            return current.Data;
        }

        public List<ServiceRequest> ToList()
        {
            var list = new List<ServiceRequest>();
            foreach (var r in this)
                list.Add(r);
            return list;
        }

        public void FromList(List<ServiceRequest> list)
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

        public IEnumerator<ServiceRequest> GetEnumerator()
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

    public class ServiceRequestCollectionConverter : JsonConverter<ServiceRequestCollection>
    {
        public override void WriteJson(JsonWriter writer, ServiceRequestCollection value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value?.ToList());
        }

        public override ServiceRequestCollection ReadJson(JsonReader reader, Type objectType, ServiceRequestCollection existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var list = serializer.Deserialize<List<ServiceRequest>>(reader);
            var collection = new ServiceRequestCollection();
            collection.FromList(list);
            return collection;
        }
    }
}

