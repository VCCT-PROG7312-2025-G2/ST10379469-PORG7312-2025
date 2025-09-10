using MunicipalServicesApp.Models.CustomCollections;

namespace MunicipalServicesApp.Models
{
    public class UserPreference
    {
        public string UserId { get; set; }
        public CustomDictionary<string, bool> PreferredLanguages { get; set; }
        public bool AllowPushNotifications { get; set; }
        public bool AllowSMSNotifications { get; set; }
        public CustomDictionary<NotificationCategory, bool> SubscribedCategories { get; set; }

        public UserPreference()
        {
            PreferredLanguages = new CustomDictionary<string, bool>();
            SubscribedCategories = new CustomDictionary<NotificationCategory, bool>();
        }
    }
}