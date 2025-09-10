using MunicipalServicesApp.Models.CustomCollections;

namespace MunicipalServicesApp.Models.ViewModels
{
    public class DashboardViewModel
    {
        public NotificationCollection Notifications { get; set; }
        public ServiceRequestCollection ServiceRequests { get; set; }
    }
}
