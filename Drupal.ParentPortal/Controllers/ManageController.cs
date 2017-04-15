using Drupal.ParentPortal.Models;
using Drupal.ParentPortal.Repository;
using Drupal.ParentPortal.Repository.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Drupal.ParentPortal.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        // GET: Manage
        public ActionResult Index()
        {
            ViewBag.UserId = "1";
            return View();
        }

        public ActionResult Alerts()
        {
            return View();
        }

        public ActionResult Documents(bool? documentUploadSucceeded, string failedMsg)
        {
            var vm = new Models.DocumentsViewModel();
            vm.Documents = Repository.Documents.GetAll();
            vm.SPDocumentLibraries = Repository.DocumentLibrary.GetAll();
            vm.Audiences = Repository.Audience.GetAll();
            vm.DocumentUploadSucceeded = documentUploadSucceeded;
            vm.DocumentUploadFailedMsg = failedMsg;
            var svm = new Models.DocumentSettingsViewModel();
            string _moduleName = Repository.Documents.ModuleName;
            svm.ConnectionString = Repository.ConfigurationItemRepository.GetUniqueItem(_moduleName, "ConnectionString");
            svm.AccountName = Repository.ConfigurationItemRepository.GetUniqueItem(_moduleName, "AccountName");
            svm.AccessKey = Repository.ConfigurationItemRepository.GetUniqueItem(_moduleName, "AccessKey");
            vm.Settings = svm;
            return View(vm);
        }
        public ActionResult Events()
        {
            var v = new Models.ManageEventViewModel();
            string _moduleName = Repository.SchoolEvent.ModuleName;// "Events";
            v.SmtpServer = Repository.ConfigurationItemRepository.GetUniqueItem(_moduleName, "SmtpServer");
            v.SmtpLoginUser = Repository.ConfigurationItemRepository.GetUniqueItem(_moduleName, "SmtpLoginUser");
            v.SmtpLoginPassword = Repository.ConfigurationItemRepository.GetUniqueItem(_moduleName, "SmtpLoginPassword");
            v.SendFromEmailAddress = Repository.ConfigurationItemRepository.GetUniqueItem(_moduleName, "SendFromEmailAddress");
            //v.Audiences = Repository.Audience.GetAudienceSelectItems();
            return View(v);
        }

        public ActionResult Office()
        {
            var vm = new OfficeViewModel();
            ConfigurationItem item = null;
            IEnumerable<ConfigurationItem> items = Repository.ConfigurationItemRepository.GetAll();

            if (items != null)
            { 
                item = items.Where(x => x.Module == "Office" && x.Key == "OfficeURL").FirstOrDefault();
            }

            if (item == null || item.ConfigurationItemId == 0)
            {
                item = new ConfigurationItem();
                item.Key = "OfficeURL";
                item.Module = "Office";
            }

            vm.ConfigItem = item;
            return View(vm);
        }

        public async Task<FileStreamResult> Download(long documentId)
        {
            Models.Document document = Repository.Documents.Get(documentId);

            AzureBlobStorage azure = new AzureBlobStorage();
            using (MemoryStream memStream = new MemoryStream())
            {
                MemoryStream output = await azure.DownloadSingleFileAsync(document);                
                byte[] file = memStream.ToArray();
                output.Write(file, 0, file.Length);
                output.Position = 0;

                HttpContext.Response.AddHeader("content-disposition", "attachment; filename=" + document.Name);

                return File(output, MIMEAssistant.GetMIMEType(document.Name)); 
            }
        } 

        public ActionResult MyStudents()
        {
            var vm = new ManageMyStudentsViewModel();
            string _moduleName = Repository.Students.ModuleName;
            ConfigurationItem configItem = Repository.ConfigurationItemRepository.GetUniqueItem(_moduleName, "StudentValidationWebService");
            vm.ConfigItem = configItem;
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveMyStudentSettings(ManageMyStudentsViewModel model)
        {

            if (!ModelState.IsValid) return null;// do what in this case? //return View(model);

            Int32 result = Repository.ConfigurationItemRepository.Save(model.ConfigItem);
            return Json(new
            {
                success = true,
                message = string.Format("Web Service URL was saved successfully.", "model"),
                //content = this.RenderPartialViewToString("MyStudents", (ManageMyStudentsViewModel)model)
            });

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveOffice(Models.OfficeViewModel input)
        {
            if ((input != null) && (input.ConfigItem != null) && (TryValidateModel(input)) && (ModelState.IsValid))
            {
                // If ?NoChrome=true is not provided, add it to the end of the url
                if (input.ConfigItem != null && !String.IsNullOrEmpty(input.ConfigItem.Value))
                {
                    input.ConfigItem = AppendNoChromeIfNotSpecified(input.ConfigItem);
                }
                
                var x = Repository.ConfigurationItemRepository.Save(input.ConfigItem);

                return Json(new
                {
                    success = true,
                    message = string.Format("URL saved.", "model"),
                    content = this.RenderPartialViewToString("Office", (OfficeViewModel)input)
                });
            }

            //return Content("URL saved.");

            //return PartialView("Office", (Models.OfficeViewModel)input);

            return Json(new
            {
                success = false,
                message = string.Format("URL was not saved.", "model"),
                content = this.RenderPartialViewToString("Office", (Models.OfficeViewModel)input)
            });
        }

        [HttpPost]
        public async Task<ActionResult> SaveDocumentLibrary(Models.DocumentsViewModel input)
        {
            try
            {
                if ((input != null) && (input.NewSPDocumentLibrary != null) && (TryValidateModel(input)) && (ModelState.IsValid))
                {

                    var docLib = input.NewSPDocumentLibrary;

                    // Save document library to db
                    docLib.DocumentLibraryId = Repository.DocumentLibrary.Save(docLib);

                    // Copy all documents from document library to Azure storage.. this may take a while if the files are large or there are many files...
                    SharePoint sp = new SharePoint();
                    List<Document> documents = await sp.DownloadFilesToAzure(docLib);

                    // Save documents that were downloaded from SharePoint and saved to Azure Blob Storage to SQL db
                    if (documents != null)
                    {
                        foreach (Document d in documents)
                        {
                            long docId = Repository.Documents.Save(d);
                        }
                    }
                    // Return successful response
                    return Json(new
                    {
                        Success = true,
                        DocumentLibraryId = docLib.DocumentLibraryId
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch(Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    message = "An internal error occurred - please try again.  If this error persists, please contact your system administrator",
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                Success = false,
                message = "Doc lib was not saved - enter required values.",
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateDocumentLibrary(Models.DocumentsViewModel input)
        {
            try
            {
                if ((input != null) && (input.EditSPDocumentLibrary != null) && (TryValidateModel(input)) && (ModelState.IsValid))
                {

                    var docLib = input.EditSPDocumentLibrary;

                    // Update document library settings to db
                    docLib.DocumentLibraryId = Repository.DocumentLibrary.Save(docLib);

                    // Return successful response
                    return Json(new
                    {
                        Success = true,
                        DocumentLibraryId = docLib.DocumentLibraryId
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    message = "An internal error occurred - please try again.  If this error persists, please contact your system administrator",
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                Success = false,
                message = "Doc lib was not saved - enter required values.",
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> RemoveDocumentLibrary(string documentlibraryid)
        {
            try
            {
                if (documentlibraryid != null)
                {
                    Models.DocumentLibrary lib = new Models.DocumentLibrary() { DocumentLibraryId = Convert.ToInt64(documentlibraryid) };

                    // delete files from Azure
                    AzureBlobStorage azureStorage = new AzureBlobStorage();
                    string error = await azureStorage.DeleteDocumentLibraryDocumentsAsync(documentlibraryid, true);

                    // delete documents for this document library from db
                    List<Models.Document> documents = Repository.Documents.GetAllFromDocumentLibrary(Convert.ToInt64(documentlibraryid));

                    foreach (Document d in documents)
                    {
                        var retVal = Repository.Documents.Delete(d);
                    }

                    // delete document library from db
                    long libId = Repository.DocumentLibrary.Delete(lib);

                    // return successful response
                    return Json(new
                    {
                        Success = true,
                        DocumentLibraryId = Convert.ToInt64(documentlibraryid)
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch(Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    message = "An internal error occurred - please try again.  If this error persists, please contact your system administrator."
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                Success = false,
                message = "documentlibraryid is required."
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> SyncDocumentLibrary(string documentlibraryid) 
        {
            try
            {
                if (documentlibraryid != null)
                {
                    Models.DocumentLibrary docLib = Repository.DocumentLibrary.Get(Convert.ToInt64(documentlibraryid));

                    // delete files from Azure
                    AzureBlobStorage azureStorage = new AzureBlobStorage();
                    string error = await azureStorage.DeleteDocumentLibraryDocumentsAsync(documentlibraryid, false);

                    // delete documents for this document library from db
                    List<Models.Document> documents = Repository.Documents.GetAllFromDocumentLibrary(Convert.ToInt64(documentlibraryid));

                    foreach (Document d in documents)
                    {
                        var retVal = Repository.Documents.Delete(d);
                    }

                    // download documents from SharePoint to Azure Storage... this might take a while if files are large in size or number
                    SharePoint sp = new SharePoint();
                    List<Document> syncedDocuments = await sp.DownloadFilesToAzure(docLib);

                    // Save documents that were downloaded from SharePoint and saved to Azure Blob Storage to SQL db
                    foreach (Document d in syncedDocuments)
                    {
                        long docId = Repository.Documents.Save(d);
                    }

                    // return successful response
                    return Json(new
                    {
                        Success = true,
                        DocumentLibraryId = Convert.ToInt64(documentlibraryid)
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    message = "An internal error occurred - please try again.  If this error persists, please contact your system administrator."
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                Success = false,
                message = "documentlibraryid is required."
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> UploadDocument([Bind(Include = "NewDocument")]DocumentsViewModel submittedVM, HttpPostedFileBase file)
        {
            bool documentUploadSucceeded = false;
            string failMsg = null;

            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    // Validate file size here... don't let bigger than 10 MB be uploaded (Azure max is 30 MB, but that's pretty big)
                    if (file.ContentLength > 10000000)
                    {
                        documentUploadSucceeded = false;
                        failMsg = "The file is too large to be uploaded to Azure.  Max file size is 10 MB.";
                    }
                    else
                    {
                        // Upload to Azure File Storage
                        AzureBlobStorage azureBlob = new AzureBlobStorage();
                        AzureFile azureFile = await azureBlob.UploadDocumentAsync(file, submittedVM.NewDocument);

                        // Save Document information to database
                        Document doc = new Document();
                        doc.Name = Path.GetFileName(file.FileName);
                        doc.Url = azureFile.Document.Url;
                        doc.Uploaded = DateTime.Now;
                        doc.Extension = Path.GetExtension(file.FileName);
                        if (submittedVM.NewDocument != null && submittedVM.NewDocument.AudienceId != null)
                        {
                            doc.AudienceId = submittedVM.NewDocument.AudienceId;
                        }

                        long saved = Repository.Documents.Save(doc);
                        documentUploadSucceeded = saved < 1 ? false : true;
                        failMsg = "Document could not be uploaded due to an internal error.  Please contact your system administrator.";
                    }
                }
                else
                {
                    documentUploadSucceeded = false;
                    failMsg = "A valid file is required to submit the form.";
                }
            }
            catch (Exception ex)
            {
                // TODO: wire this up on the front end - show a modal when the form reloads.
                documentUploadSucceeded = false;
                failMsg = "Document could not be uploaded due to an internal error.  Please contact your system administrator.";
            }

            return RedirectToAction("Documents", new { documentUploadSucceeded = documentUploadSucceeded, failMsg = failMsg });
            
        }

        [HttpPost]
        public async Task<ActionResult> RemoveFile(string documentid)
        {
            try
            {
                if (documentid != null)
                {
                    // Pull document data from db
                    Models.Document doc = Repository.Documents.Get(Convert.ToInt64(documentid));

                    // delete file from Azure
                    AzureBlobStorage azureStorage = new AzureBlobStorage();
                    await azureStorage.DeleteSingleFileAsync(doc);

                    // delete document from db
                    var retVal = Repository.Documents.Delete(doc);

                    // return successful response
                    return Json(new
                    {
                        Success = true,
                        DocumentId = documentid
                    }, JsonRequestBehavior.AllowGet);
                }

                // return successful response
                return Json(new
                {
                    Success = false,
                    message = "documentid is null"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    message = "An internal error occurred - please try again.  If this error persists, please contact your system administrator."
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UpdateDocument(Models.DocumentsViewModel input)
        {
            try
            {
                if ((input != null) && (input.EditDocument != null) && input.EditDocument.Name != null && input.EditDocument.DocumentId != 0)
                {
                    var editDocument = input.EditDocument;
                    var currentDocument = Repository.Documents.Get(editDocument.DocumentId);

                    // copy other settings that are not editable
                    editDocument.DocumentLibraryId = currentDocument.DocumentLibraryId;
                    editDocument.Uploaded = currentDocument.Uploaded;
                    editDocument.Url = currentDocument.Url;
                    editDocument.Extension = currentDocument.Extension;

                    // Update document library settings to db
                    var id = Repository.Documents.Save(editDocument);

                    // Return successful response
                    return Json(new
                    {
                        Success = true,
                        DocumentId = editDocument.DocumentId
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    message = "An internal error occurred - please try again.  If this error persists, please contact your system administrator",
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                Success = false,
                message = "Document was not saved - enter required values.",
            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult DocumentsChangesSaved(DocumentsViewModel input)
        {
            return View(input);
        }

        public ActionResult EventDetails(long? id)
        {
            if (id == null)
            {
                var sevent = Repository.SchoolEvent.Get(0);
                var model = new Models.ManageEventViewModel();
                model.SchoolEvent = sevent;
                model.Audiences = Repository.Audience.GetAudienceSelectItems();
                return PartialView("_EventDetails", model);
            }
            else
            {
                long newId = id ?? default(long);
                var sevent = Repository.SchoolEvent.Get(newId);
                var model = new Models.ManageEventViewModel();
                model.SchoolEvent = sevent;
                model.Audiences = Repository.Audience.GetAudienceSelectItems();
                return PartialView("_EventDetails", model);
            }
        }

        [HttpPost]
        public ActionResult GetEvents(DateTime start, DateTime end)
        {
            var data = Repository.SchoolEvent.GetAll()
            .Where(i => start <= i.Start && i.End <= end).ToList();


            //return Json(data);
            var ret = Json(data.Select(i => new
            {
                title = i.Title,
                start = i.Start,
                end = i.End,
                allDay = false,
                description = i.Description,
                schooleventid = i.SchoolEventId,
                location = i.Location,
                primarycontactsemail = i.PrimaryContactsEmail,
                isvolunteeropportunity = i.IsVolunteerOpportunity,
                maxvolunteers = i.MaxVolunteers,
                registeredvolunteers = i.RegisteredVolunteers,
                actionlink = i.ActionLink,
                actionbuttontext = i.ActionButtonText
            }));
            return ret;
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveEvent(Models.ManageEventViewModel input)
        {
            try
            {
                if ((input != null) && (input.SchoolEvent != null) && (TryValidateModel(input)) && (ModelState.IsValid))
                {
                    // save changes, return new data row  
                    // status code is something in 200-range
                    bool notify = input.NotifyVolunteersUpdate;
                    var x = Repository.SchoolEvent.Save(input.SchoolEvent, notify, input.CustomMessageUpdate);
                    return Json(new
                    {
                        success = true,
                        message = string.Format("Event saved.", "model")
                    });
                    //return PartialView("_EventDetails", (Models.ManageEventViewModel)input);
                }

                // set the "error status code" that will redisplay the modal
                //Response.StatusCode = 400;
                // and return the edit form, that will be displayed as a 
                // modal again - including the modelstate errors!

                return Json(new
                {
                    success = false,
                    message = string.Format("Event not saved.", "model"),
                    content = this.RenderPartialViewToString("_EventDetails", (Models.ManageEventViewModel)input)
                });
                //return PartialView("_EventDetails", (Models.ManageEventViewModel)input);
            }catch(Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = string.Format("Event not saved.", "model"),
                    content = ex.Message,
                    inputVal = input
                });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DeleteEvent(string id, string notify, string customMessage)
        {
            var seid = long.Parse(id);
            Models.SchoolEvent se = Repository.SchoolEvent.Get(seid);
            var sendNotifycation = string.IsNullOrEmpty(notify) ? false : bool.Parse(notify);
            var x = Repository.SchoolEvent.Delete(se, sendNotifycation, customMessage);
            return Json(new
            {
                success = true,
                message = string.Format("Event deleted.", "model"),
                content = this.RenderPartialViewToString("_EventDetails",null)// (Models.ManageEventViewModel)input)
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveEventSettings(Models.ManageEventViewModel input)
        {
            if ((input != null) && (TryValidateModel(input)) && (ModelState.IsValid))
            {
                var x1 = Repository.ConfigurationItemRepository.Save(input.SmtpServer);
                var x2 = Repository.ConfigurationItemRepository.Save(input.SmtpLoginUser);
                var x3 = Repository.ConfigurationItemRepository.Save(input.SmtpLoginPassword);
                var x4 = Repository.ConfigurationItemRepository.Save(input.SendFromEmailAddress);
                return Json(new
                {
                    success = true,
                    message = string.Format("Settings saved.", "model")
                });
                //return PartialView("_EventDetails", (Models.ManageEventViewModel)input);
            }

            // set the "error status code" that will redisplay the modal
            //Response.StatusCode = 400;
            // and return the edit form, that will be displayed as a 
            // modal again - including the modelstate errors!

            return Json(new
            {
                success = false,
                message = string.Format("Settings not saved.", "model")
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveDocumentSettings(Models.DocumentSettingsViewModel input)
        {
            if ((input != null) && (TryValidateModel(input)) && (ModelState.IsValid))
            {
                var x1 = Repository.ConfigurationItemRepository.Save(input.ConnectionString);
                var x2 = Repository.ConfigurationItemRepository.Save(input.AccountName);
                var x3 = Repository.ConfigurationItemRepository.Save(input.AccessKey);
                return Json(new
                {
                    success = true,
                    message = string.Format("Settings saved.", "model")
                });
                //return PartialView("_EventDetails", (Models.ManageEventViewModel)input);
            }

            // set the "error status code" that will redisplay the modal
            //Response.StatusCode = 400;
            // and return the edit form, that will be displayed as a 
            // modal again - including the modelstate errors!

            return Json(new
            {
                success = false,
                message = string.Format("Settings not saved.", "model")
            });
        }

        //[HttpPost, ValidateAntiForgeryToken]
        //public ActionResult DeleteEvent(Models.ManageEventViewModel input)
        //{
        //    var x = Repository.SchoolEvent.Delete(input.SchoolEvent);
        //    return Json(new
        //    {
        //        success = true,
        //        message = string.Format("Event deleted.", "model"),
        //        content = this.RenderPartialViewToString("_EventDetails", (Models.ManageEventViewModel)input)
        //    });
        //}

        //*****Async isn't working but don't know why!
        //
        //[HttpPost, ValidateAntiForgeryToken]
        //public async Task<ActionResult> SaveEvent(Models.ManageEventViewModel input)
        //{
        //    if ((input != null) && (input.SchoolEvent != null) && (TryValidateModel(input)))
        //    {
        //        // save changes, return new data row  
        //        // status code is something in 200-range
        //        var x = await Repository.SchoolEvent.SaveAsync(input.SchoolEvent);// db.Entry(input).State = EntityState.Modified;
        //        //await db.SaveChangesAsync();
        //        return PartialView("_EventDetails", (Models.ManageEventViewModel)input);
        //    }

        //    // set the "error status code" that will redisplay the modal
        //    Response.StatusCode = 400;
        //    // and return the edit form, that will be displayed as a 
        //    // modal again - including the modelstate errors!
        //    return PartialView("_EventDetails", (Models.ManageEventViewModel)input);
        //}

        #region Private Helper Methods
        /// <summary>
        /// Renders the partial view as a string.
        /// </summary>
        /// <param name="viewName">Name of the view.</param>
        /// <param name="model">The model.</param>
        /// <returns>A string containing the rendered HTML.</returns>
        public string RenderPartialViewToString(string viewName, object model)
        {
            this.ViewData.Model = model;
            try
            {
                using (StringWriter sw = new StringWriter())
                {
                    ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, viewName);
                    ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
                    viewResult.View.Render(viewContext, sw);

                    return sw.GetStringBuilder().ToString();
                }
            }
            catch (System.Exception ex)
            {
                return ex.ToString();
            }
        }

        private ConfigurationItem AppendNoChromeIfNotSpecified(ConfigurationItem configItem)
        {
            string url = configItem.Value;

            if (!url.Contains("?NoChrome=true"))
            {
                if (url.Contains("?"))
                {
                    configItem.Value += "&NoChrome=true";
                }
                else
                {
                    configItem.Value += "?NoChrome=true";
                }
            }

            return configItem;
        }
        #endregion
    }
}