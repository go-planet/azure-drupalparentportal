using Drupal.ParentPortal.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Converters;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Drupal.ParentPortal.Repository;
using Drupal.ParentPortal.Repository.Utilities;

namespace Drupal.ParentPortal.Controllers
{
    public class ModulesController : Controller
    {
        // GET: Modules
        public ActionResult Index(string clientId, string secret, string userId)
        {

            ViewBag.UserId = userId; //"1";
            //Validate clientId & secret
            if ((!Repository.AppManagement.IsValidUser(userId)) || (!Repository.AppManagement.IsValidClientSecret(clientId, secret)))
            {
                return View("_ModuleUnavailable");
            }
            return View();
        }

        public ActionResult Event(string clientId, string secret, string userId)
        {
            //Validate clientId & secret
            if ((!Repository.AppManagement.IsValidUser(userId)) || (!Repository.AppManagement.IsValidClientSecret(clientId, secret)))
            {
                return View("_ModuleUnavailable", model: "Events");
            }

            var vm = new EventViewModel();
            if (!string.IsNullOrEmpty(userId))
            {
                vm.UpcomingVolunteerEvents = Repository.SchoolEvent.GetUpcomingVolunteerEvents(userId);
            }
            vm.VolunteerInfo = new EventVolunteer();

            return View(vm);
        }

        public ActionResult Office(string clientId, string secret, string userId)
        {
            //Validate clientId & secret
            if ((!Repository.AppManagement.IsValidUser(userId)) || (!Repository.AppManagement.IsValidClientSecret(clientId, secret)))
            {
                return View("_ModuleUnavailable", model: "Microsoft Office");
            }
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
                
        public ActionResult Document(string clientId, string secret, string userId)
        {
            //Validate clientId & secret
            if ((!Repository.AppManagement.IsValidUser(userId)) || (!Repository.AppManagement.IsValidClientSecret(clientId, secret)))
            {
                return View("_ModuleUnavailable", model:"Documents");
            }

            return PartialView();
        }

        [HttpGet]
        public ActionResult Documents(string clientId, string secret, string userId, string page)
        {
            try
            {
                //Validate clientId & secret
                if ((!Repository.AppManagement.IsValidUser(userId)) || (!Repository.AppManagement.IsValidClientSecret(clientId, secret)))
                {
                    throw (new Exception("Validation failed."));
                }
                int p;
                int.TryParse(page, out p);
                List<Document> docs = Repository.Documents.GetByPage(userId, p);
                if (docs != null) { docs = docs.OrderBy(x => x.Name).ToList(); }
                //return PartialView(docs);
                return Json(new { Success = true, Message = docs }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet); // TODO hide exception
            }
        }

        [HttpGet]
        public ActionResult GetDocumentCount(string clientId, string secret, string userId)
        {
            try {
                //Validate clientId & secret
                if ((!Repository.AppManagement.IsValidUser(userId)) || (!Repository.AppManagement.IsValidClientSecret(clientId, secret)))
                {
                    throw (new Exception("Validation failed."));
                }
                int count = Repository.Documents.GetCount(userId);
                return Json(new { Success = true, Message = count }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message  }, JsonRequestBehavior.AllowGet); // TODO hide exception
            }
        }

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult MyStudents(string clientId, string secret, string userId)
        {

            //Validate clientId & secret
            if ((!Repository.AppManagement.IsValidUser(userId)) || (!Repository.AppManagement.IsValidClientSecret(clientId, secret)))
            {
                return View("_ModuleUnavailable", model: "My Students");
            }
            
            List<Student> s = Repository.Students.GetByParent(userId);
            var v = new MyStudentsViewModel();
            //v.ClientId = clientId;//remove once integrated with Drupal
            //v.Secret = secret;//remove once integrated with Drupal
            //v.LoggedInUser = userId;//remove once integrated with Drupal
            v.MyStudents = s;
            List<SelectListItem> grades = new List<SelectListItem>();
            grades.Add(new SelectListItem() { Text = "", Value = "" });
            grades.Add(new SelectListItem() { Text = "Kindergarten", Value = "Kindergarten" });
            grades.Add(new SelectListItem() { Text = "1st Grade", Value = "1st Grade" });
            grades.Add(new SelectListItem() { Text = "2nd Grade", Value = "2nd Grade" });
            grades.Add(new SelectListItem() { Text = "3rd Grade", Value = "3rd Grade" });
            grades.Add(new SelectListItem() { Text = "4th Grade", Value = "4th Grade" });
            grades.Add(new SelectListItem() { Text = "5th Grade", Value = "5th Grade" });
            grades.Add(new SelectListItem() { Text = "6th Grade", Value = "6th Grade" });
            grades.Add(new SelectListItem() { Text = "7th Grade", Value = "7th Grade" });
            grades.Add(new SelectListItem() { Text = "8th Grade", Value = "8th Grade" });
            grades.Add(new SelectListItem() { Text = "9th Grade", Value = "9th Grade" });
            grades.Add(new SelectListItem() { Text = "10th Grade", Value = "10th Grade" });
            grades.Add(new SelectListItem() { Text = "11th Grade", Value = "11th Grade" });
            grades.Add(new SelectListItem() { Text = "12th Grade", Value = "12th Grade" });
            v.Grades = grades;
            return View(v);
        }

        [HttpPost]
        public ActionResult SaveStudent(string clientId, string secret, string userId, string values)
        {
            //Validate clientId & secret
            if ((!Repository.AppManagement.IsValidUser(userId)) || (!Repository.AppManagement.IsValidClientSecret(clientId, secret)))
            {
                return Json(new { Success = false, Message = "Validation failed." });
            }

            var model = JsonConvert.DeserializeObject<Student>(values);
            model.ParentUserId = userId;

            /* if (string.IsNullOrEmpty(model.NewStudent.ParentUserId))
            {
                model.NewStudent.ParentUserId = loggedInUser;
                ModelState["NewStudent.ParentUserId"].Errors.Clear();
            }
            */

            //Todo: verify module has been configured with web service URL before proceeding
            try
            {
                string _moduleName = Repository.Students.ModuleName;
                string uri = Repository.ConfigurationItemRepository.GetValue(_moduleName, "StudentValidationWebService") + "?";
                //var handler = new HttpClientHandler() { Credentials = cred, CookieContainer = cookies };
                var client = new HttpClient(); // (handler);
                client.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");
                client.DefaultRequestHeaders.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
                if (!String.IsNullOrEmpty(model.StudentsSchoolId)) { uri = uri + "studentid=" + model.StudentsSchoolId; }
                if (!String.IsNullOrEmpty(model.School)) { uri = uri + "&school=" + model.School; }
                if (model.DoB != null) { uri = uri + "&dob=" + model.DoB.ToString(); }
                if (!String.IsNullOrEmpty(model.GradeLevel)) { uri = uri + "&grade=" + model.GradeLevel; }
                if (!String.IsNullOrEmpty(model.FirstName)) { uri = uri + "&firstname=" + model.FirstName; }
                if (!String.IsNullOrEmpty(model.LastName)) { uri = uri + "&lastname=" + model.LastName; }
                if (!String.IsNullOrEmpty(model.Teacher)) { uri = uri + "&teacher=" + model.Teacher; }
                string res = client.GetStringAsync(uri).Result;
                var response = res;

                dynamic data = JsonConvert.DeserializeObject<dynamic>(response);
                //var jsp = new JsonParser() { CamelizeProperties = true };
                //dynamic json = jsp.Parse(jsonText);

                //var jss = new JavaScriptSerializer();
                //var data = jss.Deserialize<dynamic>(response);
                if (data != null)
                {
                    if (data["Success"] != null)
                    {
                        bool success = data.Success;
                        if (!success)
                        {
                            throw new Exception("Validation service responded that supplied information did not match a valid student.");
                        }
                        else
                        {
                            if (data.StudentId != null) { model.StudentsSchoolId = data.StudentId; }
                            if (data.DoB != null) { model.DoB = data.DoB; }
                            if (data.GradeLevel != null) { model.GradeLevel = data.Grade; }
                            if (data.FirstName != null) { model.FirstName = data.FirstName; }
                            if (data.LastName != null) { model.LastName = data.LastName; }
                            if (data.Teacher != null) { model.Teacher = data.Teacher; }
                            model.IsValidated = true;
                        }
                    }
                }
            }catch(Exception ex)
            {
                var ret = Json(new
                {
                    Success = false,
                    Message = string.Format("Student information provided could not be validated.", "model"),
                    ErrorMessage = ex.Message
                });
                return ret;
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (!ModelState.IsValid) return Json(new { Success = false, Message = "ModelState is invalid."});

            Int32 result = Repository.Students.SaveStudent(model);
          //  var vm = new MyStudentsViewModel();
           // vm.MyStudents = Repository.Students.GetByParent(userId);
            //vm.NewStudent = new Student();
            return Json(new
            {
                Success = true,
                Message = string.Format("Student added.", "model"),
               // Content = this.RenderPartialViewToString("_MyStudents", model.MyStudents)
            });
            //return PartialView("_MyStudents",model.MyStudents);//Json(new { Success = true, Message = "Student saved." });// Content("Your student was added successfully: " + model.FirstName + "" + model.LastName);

        }
        
        [HttpPost]
        public ActionResult AddVolunteer(string clientId, string secret, string userId, string values)
        {
            try
            {
                // Validate clientId & secret
                if ((!Repository.AppManagement.IsValidUser(userId)) || (!Repository.AppManagement.IsValidClientSecret(clientId, secret)))
                {
                    return Json(new { Success = false, Message = "Validation failed." });
                }
                // Convert values to model
                var model = JsonConvert.DeserializeObject<EventVolunteer>(values);
                model.UserId = userId;

                if (!ModelState.IsValid) return Json(new { Success = false, Message = "Invalid state." });// do what in this case? //return View(model);

                Int32 result = Repository.SchoolEvent.SaveVolunteer(model);

                return Json(new { Success = true, Message = "This item was added successfully: " + model.FirstName + "," + model.LastName });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message }); // TODO hide exception
            }
            
        }

        [HttpPost]
        public ActionResult UnVolunteer(string clientId, string secret, string userId, string eventId)
        {
            try
            {
                // Validate clientId & secret
                if ((!Repository.AppManagement.IsValidUser(userId)) || (!Repository.AppManagement.IsValidClientSecret(clientId, secret)))
                {
                    return Json(new { Success = false, Message = "Validation failed." });
                }

                // Lookup EventVolunteer item by userId and eventId
                long ev = 0;
                try
                {
                    ev = long.Parse(eventId);
                }catch(Exception ex)
                { return Json(new { Success = false, Message = "Error occurred." }); }
                var model = Repository.SchoolEvent.GetEventVolunteer(userId, ev);
                Int32 result = -1;
                if (model != null)
                {
                    result = Repository.SchoolEvent.UnVolunteer(model);
                }

                return Json(new { Success = true, Message = "Volunteer cancelled successfully: " + model.FirstName + "," + model.LastName });

            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message }); // TODO hide exception
            }

        }

        [HttpGet]
        public ActionResult IsUserVolunteer(string clientId, string secret, string userId, long eventId)
        {
            // Validate clientId & secret
            if ((!Repository.AppManagement.IsValidUser(userId)) || (!Repository.AppManagement.IsValidClientSecret(clientId, secret)))
            {
                return Json(new { Success = false, Message = "Validation failed." }, JsonRequestBehavior.AllowGet);
            }
            bool isUserVolunteer = Repository.SchoolEvent.IsUserVolunteer(userId,eventId);

            return Json(isUserVolunteer, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetEvents(string clientId, string secret, string userId, DateTime start, DateTime end, string volunteerEventColor, string registeredVolunteerEventColor)
        {
            try
            {
                //Validate clientId & secret
                if ((!Repository.AppManagement.IsValidUser(userId)) || (!Repository.AppManagement.IsValidClientSecret(clientId, secret)))
                {
                    return Json(new { Success = false, Message = "Validation failed." }, JsonRequestBehavior.AllowGet);
                }

                // System.Web.HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");

                volunteerEventColor = String.IsNullOrEmpty(volunteerEventColor) ? "" : volunteerEventColor;
                registeredVolunteerEventColor = String.IsNullOrEmpty(registeredVolunteerEventColor) ? "" : registeredVolunteerEventColor;

                var data = Repository.SchoolEvent.GetAll(userId)
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
                    actionbuttontext = i.ActionButtonText,
                    color = (i.Volunteers.FirstOrDefault(v => v.UserId == userId) != null) ? registeredVolunteerEventColor : i.IsVolunteerOpportunity ? volunteerEventColor : ""
                }), JsonRequestBehavior.AllowGet);
                return ret;
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet); // TODO hide exception
            }
        }

        /// <summary>
        /// This is a test end point in the absence of a real web service to validate that the info supplied by a parent for their student matches the Student Information
        /// Management system's data for a student. To test failure scenarios pass in "fail" for the school.
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="school"></param>
        /// <param name="doB"></param>
        /// <param name="grade"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="teacher"></param>
        /// <returns>Always returns true unless the school input is "fail".</returns>
        [HttpGet]
        public JsonResult VerifyStudent(string studentId, string school, DateTime doB, string grade, string firstName, string lastName, string teacher)
        {
            if (school.Trim().ToLower().Equals("fail"))
            {
                return Json(new
                {
                    Success = false
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    Success = true,
                    StudentId = studentId,
                    School = school,
                    DoB = doB,
                    Grade = grade,
                    FirstName = firstName,
                    LastName = lastName,
                    Teacher = teacher
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult RemoveStudent(string clientId, string secret, string userId, string studentId)
        {
            //Validate clientId & secret
            if ((!Repository.AppManagement.IsValidUser(userId)) || (!Repository.AppManagement.IsValidClientSecret(clientId, secret)))
            {
                return Json(new { Success = false, Message = "Validation failed." });
            }

            //Lookup EventVolunteer item by userId and eventId
            long ev = 0;
            try
            {
                ev = long.Parse(studentId);
            }
            catch (Exception ex)
            { return Json(new { Success = false, Message = "Error occurred removing the student; invalid StudentId." }); }
            Student model = Repository.Students.GetById(ev);
            if (model == null) return Json(new { Success = false, Message = "Error occurred removing the student. Student not found." });
            Int32 result = Repository.Students.RemoveStudent(model);
            if(result <1) return Json(new { Success = false, Message = "Error occurred removing the student." });
            return Json(new
            {
                Success = true,
                StudentId = studentId,
                School = model.School,
                DoB = model.DoB,
                Grade = model.GradeLevel,
                FirstName = model.FirstName,
                LastName = model.LastName
            }, JsonRequestBehavior.AllowGet);

        }

        public async Task<FileStreamResult> Download(long documentId, string clientId, string secret, string userId)
        {
            //Validate clientId & secret
            if ((!Repository.AppManagement.IsValidUser(userId)) || (!Repository.AppManagement.IsValidClientSecret(clientId, secret)))
            {
                return null;
            }

            Models.Document document = Repository.Documents.Get(documentId);

            AzureBlobStorage azure = new AzureBlobStorage();
            using (MemoryStream memStream = new MemoryStream())
            {
                MemoryStream output = await azure.DownloadSingleFileAsync(document);
                byte[] file = memStream.ToArray();
                if (output != null)
                {
                    output.Write(file, 0, file.Length);
                    output.Position = 0;

                    HttpContext.Response.AddHeader("content-disposition", "attachment; filename=" + document.Name);

                    return File(output, MIMEAssistant.GetMIMEType(document.Name));
                }
                else
                {
                    //What to do here? If Azure blob storage connection settings are not valid then this is where code will land
                    return null;
                }
            }
        }

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
        #endregion

    }
}