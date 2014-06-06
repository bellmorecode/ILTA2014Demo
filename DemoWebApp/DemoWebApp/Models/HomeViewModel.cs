using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoWebApp.Models
{
    public class HomeViewModel
    {
        public HomeViewModel()
        {
            this.Announcements = new List<Announcement>();
            this.Documents = new List<Document>();
        }
        public List<Announcement> Announcements { get; set; }
        public List<Document> Documents { get; set; }
        public List<EWSTask> Tasks { get; set; }
    }

    public class Announcement
    {
        public string Title { get; set; }
        public string Body { get; set; }
    }

    public class Document
    {
        public string Title { get; set; }
        public string Url { get; set; }
    }

    public class EWSTask
    {
        public string Subject { get; set; }
        public DateTime? DueDate { get; set; }
    }
}