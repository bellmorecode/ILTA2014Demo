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
        }
        public List<Announcement> Announcements { get; set; }
    }

    public class Announcement
    {
        public string Title { get; set; }
    }
}