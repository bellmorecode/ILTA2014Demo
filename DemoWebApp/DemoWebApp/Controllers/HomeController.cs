using DemoWebApp.Models;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
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
            #region Auth Stuff
            var url = "https://somethingdigital.sharepoint.com/sites/spdev";
            ClientContext clientContext = new ClientContext(url);   

            var user = ConfigurationManager.AppSettings["userlogin"];
            var pwd = ConfigurationManager.AppSettings["password"];

            #if PASSWORD_HACK
            // go grab the real password from my desktop
            // for testing purposes only. 
            pwd = System.IO.File.ReadAllText(@"c:\password.txt");
            #endif

            // encode password and set credentials
            SecureString passWord = new SecureString();
            foreach (char c in pwd.ToCharArray()) passWord.AppendChar(c);
            clientContext.Credentials = new SharePointOnlineCredentials(user, passWord);
            #endregion

            // Get Lists from SharePoint
            Web oWeb = clientContext.Web;
            ListCollection oListCollection = oWeb.Lists;
            clientContext.Load(oListCollection);
            clientContext.ExecuteQuery();

            //var folderlist = oListCollection.Select(q => new Document { Title = q.EntityTypeName, Url = "" }).ToList();

            // Get Items from Announcements List
            var announcementsList = oListCollection.First(q => q.EntityTypeName == "ILTA_x0020_AnnouncementsList");
            var items = announcementsList.GetItems(new CamlQuery { ViewXml = "<View><ViewFields  ><FieldRef Name='Title' /><FieldRef Name='Body' /></ViewFields></View>" });
            clientContext.Load(items);
            clientContext.ExecuteQuery();

            // Get Items from Documents List
            var docsList = oListCollection.First(q => q.EntityTypeName == "Shared_x0020_Documents");
            var docs = docsList.GetItems(new CamlQuery { ViewXml = "<View><ViewFields  ><FieldRef Name='DocIcon' /><FieldRef Name='FileRef' /><FieldRef Name='Title' /><FieldRef Name='ows_LinkFilename' /></ViewFields></View>" });
            clientContext.Load(docs);
            clientContext.ExecuteQuery();

            var tasks = GetMyTasks(user, pwd);

            // create View Model
            var model = new Models.HomeViewModel();
            model.Announcements = items.Select(q => new Announcement { Title = q.FieldValues["Title"].ToString(), Body = q.FieldValues["Body"].ToString() }).ToList();
            model.Documents = docs.Select(q => new Document { Title = q.FieldValues["FileLeafRef"].ToString(), Url = string.Format("{0}{1}", "https://somethingdigital.sharepoint.com", q.FieldValues["FileRef"].ToString())  }).ToList();
            model.Tasks = tasks.Items.Select(q => (q as Task)).Select(q => new EWSTask { Subject = q.Subject, DueDate = q.DueDate }).ToList();
            return View(model);
        }

        private FindItemsResults<Item> GetMyTasks(string user, string pwd)
        {
            // reference: http://msdn.microsoft.com/EN-US/library/office/dn567668(v=exchg.150).aspx
            // reference: http://code.msdn.microsoft.com/exchange/Exchange-2013-101-Code-3c38582c
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013_SP1);
            service.Credentials = new WebCredentials(user, pwd);

            service.TraceEnabled = false;
            //service.TraceFlags = TraceFlags.All;

            service.Url = new Uri("https://outlook.office365.com/EWS/Exchange.asmx");
            // OPTIONALLY: WE CAN AUTO-DISCOVER (SEE BELOW)
            //service.AutodiscoverUrl(user, RedirectionUrlValidationCallback);

            TasksFolder tasksfolder = TasksFolder.Bind(service,
                                                       WellKnownFolderName.Tasks,
                                                       new PropertySet(BasePropertySet.IdOnly, FolderSchema.TotalCount));

            // Set the number of items to the smaller of the number of items in the Contacts folder or 1000.
            int numItems = tasksfolder.TotalCount < 1000 ? tasksfolder.TotalCount : 1000;

            // Instantiate the item view with the number of items to retrieve from the contacts folder.
            ItemView view = new ItemView(numItems);

            // To keep the request smaller, send only the display name.
            view.PropertySet = new PropertySet(BasePropertySet.IdOnly, TaskSchema.Subject, TaskSchema.DueDate);

            // Create a searchfilter to check the subject of the tasks.
            SearchFilter.IsGreaterThanOrEqualTo filter = new SearchFilter.IsGreaterThanOrEqualTo(TaskSchema.DueDate, DateTime.Now.Date);

            // Retrieve the items in the Tasks folder with the properties you selected.
            FindItemsResults<Item> taskItems = service.FindItems(WellKnownFolderName.Tasks, filter, view);

            return taskItems;
        }

        private static bool RedirectionUrlValidationCallback(string redirectionUrl)
        {
            // The default for the validation callback is to reject the URL.
            bool result = false;

            Uri redirectionUri = new Uri(redirectionUrl);

            // Validate the contents of the redirection URL. In this simple validation
            // callback, the redirection URL is considered valid if it is using HTTPS
            // to encrypt the authentication credentials. 
            if (redirectionUri.Scheme == "https")
            {
                result = true;
            }
            return result;
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