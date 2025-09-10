using System;
using MunicipalServicesApp.Models.CustomCollections;
using Newtonsoft.Json;

namespace MunicipalServicesApp.Models
{
    public class ServiceRequest
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Category { get; set; }

        [JsonIgnore] 
        public CustomStack<string> MediaAttachments { get; set; }

        public DateTime CreatedDate { get; set; }
        public RequestStatus Status { get; set; }
        public string AssignedDepartment { get; set; }
        public string UserId { get; set; }

        public CustomStack<StatusUpdate> Updates { get; set; }

        public ServiceRequest()
        {
            MediaAttachments = new CustomStack<string>();
            Updates = new CustomStack<StatusUpdate>();
        }
    }
}