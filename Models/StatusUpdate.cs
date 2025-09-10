using System;

namespace MunicipalServicesApp.Models
{
    public class StatusUpdate
    {
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
        public string UpdatedBy { get; set; }
        public RequestStatus NewStatus { get; set; }
    }
}