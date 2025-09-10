using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MunicipalServicesApp.Models
{
    public class ReportIssueViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Please select a category")]
        public string Category { get; set; }

        public List<IFormFile> MediaFiles { get; set; }
        public List<string> MediaAttachments { get; set; }

        public ReportIssueViewModel()
        {
            MediaFiles = new List<IFormFile>();
            MediaAttachments = new List<string>();
        }
    }
}