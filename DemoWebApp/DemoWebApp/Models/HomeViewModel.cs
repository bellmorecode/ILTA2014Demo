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
    }

    public class Announcement
    {
        public string Title { get; set; }
    }

    public class Document
    {
        public string Title { get; set; }
        public string Url { get; set; }
    }
}