using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security;
using System.Web;
using System.Web.Mvc;

namespace DemoWebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // Get Announcements List from SharePoint Online
            // instantiate client context
            ClientContext clientContext = new ClientContext("https://somethingdigital.sharepoint.com/sites/spdev");   
             
            // encode password and set credentials
            SecureString passWord = new SecureString();
            foreach (char c in "yourpassword".ToCharArray()) passWord.AppendChar(c);
            clientContext.Credentials = new SharePointOnlineCredentials("administrator@somethingdigital.onmicrosoft.com", passWord);

            // Get Lists from SharePoint
            Web oWeb = clientContext.Web;
            ListCollection oListCollection = oWeb.Lists;
            clientContext.Load(oListCollection);
            clientContext.ExecuteQuery();

            // Get Items from Announcements List
            var announcementsList = oListCollection.First(q => q.EntityTypeName == "ILTA_x0020_AnnouncementsList");
            var items = announcementsList.GetItems(new CamlQuery { ViewXml = "<View><ViewFields  ><FieldRef Name='Title' /></ViewFields></View>" });
            clientContext.Load(items);
            clientContext.ExecuteQuery();

            // create View Model
            var model = new Models.HomeViewModel();
            model.Announcements = items.Select(q => new Models.Announcement { Title = q.FieldValues["Title"].ToString() }).ToList();

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}