using Microsoft.AspNetCore.Mvc;
using MunicipalServicesApp.Models;
using MunicipalServicesApp.Models.CustomCollections;
using MunicipalServicesApp.Models.ViewModels; // ✅ Added
using System;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace MunicipalServicesApp.Controllers
{
    public class HomeController : Controller
    {
        private const string NotificationsKey = "UserNotifications";
        private const string ServiceRequestsKey = "UserServiceRequests";
        private readonly IWebHostEnvironment _environment;

        public HomeController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        private NotificationCollection GetNotifications()
        {
            var notificationsJson = HttpContext.Session.GetString(NotificationsKey);
            if (string.IsNullOrEmpty(notificationsJson))
            {
                return CreateSampleNotifications();
            }

            try
            {
                return DeserializeNotifications(notificationsJson);
            }
            catch
            {
                return CreateSampleNotifications();
            }
        }

        private NotificationCollection CreateSampleNotifications()
        {
            var notifications = new NotificationCollection();

            notifications.Add(new Notification
            {
                Id = "1",
                Title = "Water Maintenance Scheduled",
                Message = "Scheduled water maintenance in your area on Friday from 10:00 to 14:00",
                Timestamp = DateTime.Now.AddHours(-2),
                Category = NotificationCategory.General,
                IsRead = false,
                UserId = "user123"
            });

            notifications.Add(new Notification
            {
                Id = "2",
                Title = "Load Shedding Update",
                Message = "Stage 2 load shedding implemented from 16:00 to 18:00",
                Timestamp = DateTime.Now.AddHours(-5),
                Category = NotificationCategory.Emergency,
                IsRead = false,
                UserId = "user123"
            });

            notifications.Add(new Notification
            {
                Id = "3",
                Title = "Service Request Updated",
                Message = "Your request 'Pothole Repair' has been updated: Assessment team dispatched",
                Timestamp = DateTime.Now.AddDays(-1),
                Category = NotificationCategory.ServiceUpdate,
                IsRead = true,
                UserId = "user123"
            });

            SaveNotifications(notifications);
            return notifications;
        }

        private void SaveNotifications(NotificationCollection notifications)
        {
            HttpContext.Session.SetString(NotificationsKey, SerializeNotifications(notifications));
        }

        private ServiceRequestCollection GetServiceRequests()
        {
            var requestsJson = HttpContext.Session.GetString(ServiceRequestsKey);
            if (string.IsNullOrEmpty(requestsJson))
            {
                return CreateSampleServiceRequests();
            }

            try
            {
                return DeserializeServiceRequests(requestsJson);
            }
            catch
            {
                return CreateSampleServiceRequests();
            }
        }

        private ServiceRequestCollection CreateSampleServiceRequests()
        {
            var requests = new ServiceRequestCollection();

            var request1 = new ServiceRequest
            {
                Id = "1",
                Title = "Pothole Repair",
                Description = "Large pothole on Main Street causing traffic issues",
                Location = "Main Street, City Center",
                Category = "Roads",
                CreatedDate = DateTime.Now.AddDays(-2),
                Status = RequestStatus.InProgress,
                UserId = "user123"
            };
            request1.Updates.Push(new StatusUpdate
            {
                Timestamp = DateTime.Now.AddDays(-1),
                Message = "Assessment team dispatched",
                UpdatedBy = "Public Works Dept",
                NewStatus = RequestStatus.InProgress
            });
            requests.Add(request1);

            requests.Add(new ServiceRequest
            {
                Id = "2",
                Title = "Street Light Outage",
                Description = "Street light not working on Oak Avenue",
                Location = "Oak Avenue, Suburbia",
                Category = "Utilities",
                CreatedDate = DateTime.Now.AddDays(-1),
                Status = RequestStatus.Submitted,
                UserId = "user123"
            });

            SaveServiceRequests(requests);
            return requests;
        }

        private void SaveServiceRequests(ServiceRequestCollection serviceRequests)
        {
            HttpContext.Session.SetString(ServiceRequestsKey, SerializeServiceRequests(serviceRequests));
        }

        private string SerializeNotifications(NotificationCollection notifications)
        {
            var notificationList = notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Timestamp = n.Timestamp,
                Category = n.Category,
                IsRead = n.IsRead,
                UserId = n.UserId
            }).ToList();

            return JsonConvert.SerializeObject(notificationList);
        }

        private NotificationCollection DeserializeNotifications(string json)
        {
            var notificationDtos = JsonConvert.DeserializeObject<List<NotificationDto>>(json);
            var collection = new NotificationCollection();

            if (notificationDtos != null)
            {
                foreach (var dto in notificationDtos)
                {
                    collection.Add(new Notification
                    {
                        Id = dto.Id,
                        Title = dto.Title,
                        Message = dto.Message,
                        Timestamp = dto.Timestamp,
                        Category = dto.Category,
                        IsRead = dto.IsRead,
                        UserId = dto.UserId
                    });
                }
            }
            return collection;
        }

        private string SerializeServiceRequests(ServiceRequestCollection serviceRequests)
        {
            var requestList = serviceRequests.Select(r => new ServiceRequestDto
            {
                Id = r.Id,
                Title = r.Title,
                Description = r.Description,
                Location = r.Location,
                Category = r.Category,
                CreatedDate = r.CreatedDate,
                Status = r.Status,
                UserId = r.UserId,
                MediaAttachments = r.MediaAttachments.ToList(),
                Updates = r.Updates.ToList().Select(u => new StatusUpdateDto
                {
                    Timestamp = u.Timestamp,
                    Message = u.Message,
                    UpdatedBy = u.UpdatedBy,
                    NewStatus = u.NewStatus
                }).ToList()
            }).ToList();

            return JsonConvert.SerializeObject(requestList);
        }

        private ServiceRequestCollection DeserializeServiceRequests(string json)
        {
            var requestDtos = JsonConvert.DeserializeObject<List<ServiceRequestDto>>(json);
            var collection = new ServiceRequestCollection();

            if (requestDtos != null)
            {
                foreach (var dto in requestDtos)
                {
                    var request = new ServiceRequest
                    {
                        Id = dto.Id,
                        Title = dto.Title,
                        Description = dto.Description,
                        Location = dto.Location,
                        Category = dto.Category,
                        CreatedDate = dto.CreatedDate,
                        Status = dto.Status,
                        UserId = dto.UserId
                    };

                    foreach (var media in dto.MediaAttachments)
                    {
                        request.MediaAttachments.Push(media);
                    }

                    foreach (var updateDto in dto.Updates.AsEnumerable().Reverse()) // ✅ fixed
                    {
                        request.Updates.Push(new StatusUpdate
                        {
                            Timestamp = updateDto.Timestamp,
                            Message = updateDto.Message,
                            UpdatedBy = updateDto.UpdatedBy,
                            NewStatus = updateDto.NewStatus
                        });
                    }

                    collection.Add(request);
                }
            }
            return collection;
        }

        public IActionResult Index()
        {
            var model = new DashboardViewModel
            {
                Notifications = GetNotifications(),
                ServiceRequests = GetServiceRequests()
            };
            return View(model);
        }

        public IActionResult ServiceRequests()
        {
            var model = GetServiceRequests();
            return View(model);
        }

        public IActionResult Notifications()
        {
            var model = GetNotifications();
            return View(model);
        }

        public IActionResult Settings()
        {
            return View();
        }

        public IActionResult ReportIssue()
        {
            var model = new ReportIssueViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitIssue(ReportIssueViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("ReportIssue", model);
            }

            var requests = GetServiceRequests();

            var newRequest = new ServiceRequest
            {
                Id = (requests.Count + 1).ToString(),
                Title = model.Title,
                Description = model.Description,
                Location = model.Location,
                Category = model.Category,
                CreatedDate = DateTime.Now,
                Status = RequestStatus.Submitted,
                UserId = "user123"
            };

            if (model.MediaFiles != null)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                foreach (var file in model.MediaFiles)
                {
                    if (file.Length > 0)
                    {
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        newRequest.MediaAttachments.Push(uniqueFileName);
                    }
                }
            }

            requests.Add(newRequest);
            SaveServiceRequests(requests);

            var notifications = GetNotifications();
            var notification = new Notification
            {
                Id = (notifications.Count + 1).ToString(),
                Title = "Service Request Submitted",
                Message = $"Your {model.Category} request at {model.Location} has been submitted successfully.",
                Timestamp = DateTime.Now,
                Category = NotificationCategory.ServiceUpdate,
                IsRead = false,
                UserId = "user123"
            };

            notifications.Add(notification);
            SaveNotifications(notifications);

            TempData["SuccessMessage"] = "Your issue has been reported successfully!";
            return RedirectToAction("ServiceRequests");
        }

        [HttpPost]
        public IActionResult MarkAsRead(string id)
        {
            var notifications = GetNotifications();
            var notification = notifications.FindById(id);
            if (notification != null)
            {
                notification.IsRead = true;
                SaveNotifications(notifications);
            }
            return RedirectToAction("Notifications");
        }

        [HttpPost]
        public IActionResult MarkAllAsRead()
        {
            var notifications = GetNotifications();

            foreach (var notification in notifications)
            {
                if (!notification.IsRead)
                {
                    notification.IsRead = true;
                }
            }

            SaveNotifications(notifications);
            return RedirectToAction("Notifications");
        }
    }

    public class NotificationDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public NotificationCategory Category { get; set; }
        public bool IsRead { get; set; }
        public string UserId { get; set; }
    }

    public class ServiceRequestDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Category { get; set; }
        public DateTime CreatedDate { get; set; }
        public RequestStatus Status { get; set; }
        public string UserId { get; set; }
        public List<string> MediaAttachments { get; set; } = new List<string>();
        public List<StatusUpdateDto> Updates { get; set; } = new List<StatusUpdateDto>();
    }

    public class StatusUpdateDto
    {
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
        public string UpdatedBy { get; set; }
        public RequestStatus NewStatus { get; set; }
    }
}
