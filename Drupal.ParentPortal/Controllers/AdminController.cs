using Drupal.ParentPortal.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Drupal.ParentPortal.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        // GET: Admin/Edit/5
        public ActionResult Edit()
        {
            var item = GetRegistrant();
            if (item == null) item = new AppManagement();
            return View(item);
        }

        // POST: Admin/Edit/5
        [HttpPost]
        public ActionResult Edit(AppManagement am)
        {
            if (!ModelState.IsValid) return View(am);

            Int32 result = Repository.AppManagement.Save(am);
            return Json(new
            {
                Success = true,
                Message = string.Format("Settings saved successfully.", "model")
            });
            //return RedirectToAction("Edit");
        }

        private void UpdateTable(AppManagement am)
        {


            //var storage = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["Microsoft.WindowsAzure.Storage.ConnectionString"]);
            //var client = storage.CreateCloudTableClient();
            //var table = client.GetTableReference("Registrants");
            //table.CreateIfNotExists();

            //var tOperation = TableOperation.InsertOrMerge(r);
            //table.Execute(tOperation);
        }

        private Models.AppManagement GetRegistrant()
        {
            Models.AppManagement am = Repository.AppManagement.Get();
            if (am == null) am = new AppManagement();
            //var storage = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["Microsoft.WindowsAzure.Storage.ConnectionString"]);
            //var client = storage.CreateCloudTableClient();
            //var table = client.GetTableReference("Registrants");
            //table.CreateIfNotExists();

            //var query = new TableQuery<Registrant>();
            //var item = table.ExecuteQuery(query).ToList().FirstOrDefault();

            return am;
        }


    }
}
