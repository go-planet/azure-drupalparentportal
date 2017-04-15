namespace Drupal.ParentPortal.Repository
        {
            using System;
            using System.Data.Entity;
            using System.Linq;
            using System.Collections.Generic;
            using Models;

    public class SchoolEvent
    {

        public const string ModuleName = "Events";
        public static Models.SchoolEvent Get(long id)
        {
            using (var db = new Context.SqlContext())
            {
                if (id == 0) return new Models.SchoolEvent();
                return db.SchoolEvent.Include(i => i.Volunteers).FirstOrDefault<Models.SchoolEvent>(v => v.SchoolEventId.Equals(id));
            }
        }
        public static Models.SchoolEvent Get()
        {
            using (var db = new Context.SqlContext())
            {
                return db.SchoolEvent.FirstOrDefault<Models.SchoolEvent>();
            }
        }
        public static List<Models.SchoolEvent> GetAll()
        {
            try
            {
                using (var db = new Context.SqlContext())
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    var ses = db.SchoolEvent.Include(i => i.Volunteers).ToList();
                    return ses;
                }
            }catch(Exception ex)
            {
                var msg = ex.Message;
                return null;
            }
        }
        public static List<Models.SchoolEvent> GetAll(string userId)
        {
            var audiences = Repository.Audience.GetUsersAudienceIds(userId);
            using (var db = new Context.SqlContext())
            {
                //Need to load only the Volunteers navigation property for the provided user so the 
                //UI can reflect the user has already volunteered and prevent user from re-volunteering
                db.Configuration.LazyLoadingEnabled = false;
                var seList = db.SchoolEvent.Include(i => i.Volunteers).Where(i => i.AudienceId == null || audiences.Contains(i.AudienceId)).ToList();
                //if (!String.IsNullOrEmpty(userId)) db.SchoolEvent;// Reference(p => p.SchoolEvent).Load();
                
                return seList;
            }
        }

        public static int Save(Models.SchoolEvent schoolEvent, bool notifyVolunteers, string customMessage)
        {
            try
            {
                using (var db = new Context.SqlContext())
                {
                    db.Entry(schoolEvent).State = schoolEvent.SchoolEventId.Equals(0) ? EntityState.Added : EntityState.Modified;
                    var ret = db.SaveChanges();

                    if ((schoolEvent.IsVolunteerOpportunity) && (notifyVolunteers))
                    {
                        List<string> volunteerEmails = new List<string>();
                        if (schoolEvent.Volunteers == null)
                        {
                            db.Entry(schoolEvent).Collection(v => v.Volunteers).Load();
                        }
                        if (schoolEvent.Volunteers.Count > 0)
                        {
                            foreach (EventVolunteer ev in schoolEvent.Volunteers)
                            {
                                if (!string.IsNullOrEmpty(ev.Email))
                                {
                                    volunteerEmails.Add(ev.Email);
                                }
                            }
                        }
                        if ((volunteerEmails.Count > 0) || !string.IsNullOrEmpty(schoolEvent.PrimaryContactsEmail))
                        {
                            List<string> bcc = volunteerEmails;
                            List<string> to = new List<string>();
                            if (!string.IsNullOrEmpty(schoolEvent.PrimaryContactsEmail)) to.Add(schoolEvent.PrimaryContactsEmail);
                            string subject = "Updated: \"" + schoolEvent.Title + "\" (starts on " + string.Format("{0:g}", schoolEvent.Start) + ")";
                            string body = "Details regarding the \"" + schoolEvent.Title + "\" event have been updated. <br>" +
                                (string.IsNullOrEmpty(customMessage) ? "" : customMessage) + "<br>" +
                                "<b>Start</b>: " + string.Format("{0:f}", schoolEvent.Start) + "<br>" +
                                "<b>End</b>: " + string.Format("{0:f}", schoolEvent.End) + "<br>" +
                                (!string.IsNullOrEmpty(schoolEvent.Location) ? ("<b>Location</b>: " + schoolEvent.Location + "<br>") : "") +
                                (!string.IsNullOrEmpty(schoolEvent.Description) ? ("<b>Description</b>: " + schoolEvent.Description + "<br>") : "");
                            bool emailsent = SendEventEmail(to, null, bcc, subject, body);
                        }
                    }

                    return ret;

                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public static System.Threading.Tasks.Task<int> SaveAsync(Models.SchoolEvent schoolEvent)
        {

            using (var db = new Context.SqlContext())
            {
                db.Entry(schoolEvent).State = schoolEvent.SchoolEventId.Equals(0) ? EntityState.Added : EntityState.Modified;
                return db.SaveChangesAsync();
            }
        }
        public static int SaveVolunteer(Models.EventVolunteer eventVolunteer)
        {
            using (var db = new Context.SqlContext())
            {

                db.Entry(eventVolunteer).State = eventVolunteer.EventVolunteerId.Equals(0) ? EntityState.Added : EntityState.Modified;
                if (db.Entry(eventVolunteer).State == EntityState.Added)
                {
                    //Load School event
                    if(eventVolunteer.SchoolEvent == null) db.Entry(eventVolunteer).Reference(p => p.SchoolEvent).Load();
                    //Prevent new volunteer logic:
                    if (eventVolunteer.SchoolEvent.End <= DateTime.Now) return -1;//can't register for event that has already happened 
                    if (!eventVolunteer.SchoolEvent.IsVolunteerOpportunity) return -1;//can't register for event that isn't a volunteer opportunity
                    if (eventVolunteer.SchoolEvent.MaxVolunteers > 0 && eventVolunteer.SchoolEvent.RegisteredVolunteers >= eventVolunteer.SchoolEvent.MaxVolunteers) return -1;//can't register for event that has reached the max volunteers allowed
                    eventVolunteer.RegisteredDate = DateTime.Now;
                    eventVolunteer.SchoolEvent.RegisteredVolunteers++;
                    var ret = db.SaveChanges();
                    //Send email notification to primary contact and new volunteer
                    if ((!string.IsNullOrEmpty(eventVolunteer.Email)) || string.IsNullOrEmpty(eventVolunteer.SchoolEvent.PrimaryContactsEmail))
                    {
                        List<string> to = new List<string>();
                        to.Add(eventVolunteer.Email);
                        List<string> cc = new List<string>();
                        if(!string.IsNullOrEmpty(eventVolunteer.SchoolEvent.PrimaryContactsEmail)) cc.Add(eventVolunteer.SchoolEvent.PrimaryContactsEmail);
                        string subject = "You volunteered for \"" + eventVolunteer.SchoolEvent.Title + "\" (starts on " + eventVolunteer.SchoolEvent.Start + ")";
                        string body = "Thank you, " + eventVolunteer.FirstName + ", for volunteering for the \"" + eventVolunteer.SchoolEvent.Title + "\" event. <br>" +
                            "<b>Start</b>: " + string.Format("{0:f}", eventVolunteer.SchoolEvent.Start) + "<br>" +
                            "<b>End</b>: " + string.Format("{0:f}", eventVolunteer.SchoolEvent.End) + "<br>" +
                            (!string.IsNullOrEmpty(eventVolunteer.SchoolEvent.Location) ? ("<b>Location</b>: " + eventVolunteer.SchoolEvent.Location + "<br>") : "") +
                            (!string.IsNullOrEmpty(eventVolunteer.SchoolEvent.Description) ? ("<b>Description</b>: " + eventVolunteer.SchoolEvent.Description + "<br>") : "");
                        bool emailsent = SendEventEmail(to, cc, null, subject, body);
                    }

                    return ret;
                }
                else
                {
                    return db.SaveChanges();
                }
            }
        }
        public static int UnVolunteer(Models.EventVolunteer eventVolunteer)
        {
            using (var db = new Context.SqlContext())
            {
                try
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    if (eventVolunteer.EventVolunteerId.Equals(0)) return -1; //can't delete a record that hasn't been created in the db yet
                    if (eventVolunteer.SchoolEvent == null)
                    {

                        db.Entry(eventVolunteer).Reference("SchoolEvent").Load();
                    }
                    eventVolunteer.SchoolEvent.RegisteredVolunteers = eventVolunteer.SchoolEvent.RegisteredVolunteers <=0 ? 0 : eventVolunteer.SchoolEvent.RegisteredVolunteers-1;
                    db.Entry(eventVolunteer.SchoolEvent).State = EntityState.Modified;
                    db.Entry(eventVolunteer).State = EntityState.Deleted;
                    var ret = db.SaveChanges();
                    //Send email notification to primary contact the volunteer
                    if ((!string.IsNullOrEmpty(eventVolunteer.Email)) || string.IsNullOrEmpty(eventVolunteer.SchoolEvent.PrimaryContactsEmail))
                    {
                        List<string> to = new List<string>();
                        to.Add(eventVolunteer.Email);
                        List<string> cc = new List<string>();
                        if ((eventVolunteer.SchoolEvent != null) && (!string.IsNullOrEmpty(eventVolunteer.SchoolEvent.PrimaryContactsEmail))) cc.Add(eventVolunteer.SchoolEvent.PrimaryContactsEmail);
                        string subject = "Canceled: \"" + eventVolunteer.SchoolEvent.Title + "\", on " + eventVolunteer.SchoolEvent.Start;
                        string body = "Thank you, " + eventVolunteer.FirstName + ", for notifying us that you need to cancel for the \"" + eventVolunteer.SchoolEvent.Title +
                            "\" event that starts " + string.Format("{0:f}", eventVolunteer.SchoolEvent.Start) + ".";
                        bool emailsent = SendEventEmail(to, cc, null, subject, body);
                    }

                    return ret;
                }
                catch (Exception ex)
                {
                    var e = ex.Message;
                    return -1;
                }
            }
        }
        public static Models.EventVolunteer GetEventVolunteer(string userId, long schoolEventId)
        {
            using (var db = new Context.SqlContext())
            {
                var ev = db.EventVolunteer.Include(i => i.SchoolEvent).FirstOrDefault<Models.EventVolunteer>(v => v.SchoolEventId.Equals(schoolEventId) && v.UserId.Equals(userId));
                return ev;
            }
        }
        public static bool IsUserVolunteer(string userId, long schoolEventId)
        {
            bool isUserVolunteer = false;
            using (var db = new Context.SqlContext())
            {
                var evs = db.EventVolunteer.FirstOrDefault<Models.EventVolunteer>(v => v.SchoolEventId.Equals(schoolEventId) && v.UserId.Equals(userId));
                if (evs != null) isUserVolunteer = true;
            }
            return isUserVolunteer;
        }
        public static List<Models.EventVolunteer> GetUpcomingVolunteerEvents(string userId)
        {
            using (var db = new Context.SqlContext())
            {
                var r = db.EventVolunteer.Where(s => s.UserId == userId);
                var twoWeeksOut = DateTime.Now.AddDays(15);
                var evs = (from EventVolunteer in db.EventVolunteer.Include(i => i.SchoolEvent)
                             where EventVolunteer.UserId == userId && 
                             EventVolunteer.SchoolEvent.Start > DateTime.Now && EventVolunteer.SchoolEvent.End > DateTime.Now && 
                             EventVolunteer.SchoolEvent.End < twoWeeksOut
                             select EventVolunteer).OrderBy(ev => ev.SchoolEvent.Start).ToList();
                return evs;
            }
        }

        public static object Delete(Models.SchoolEvent schoolEvent, bool notifyVolunteers, string customMessage)
        {
            using (var db = new Context.SqlContext())
            {
                List<string> volunteerEmails = new List<string>();
                if (schoolEvent.Volunteers == null)
                {
                    db.Entry(schoolEvent).Collection(v => v.Volunteers).Load();
                }
                if (schoolEvent.Volunteers.Count > 0)
                {
                    if(notifyVolunteers)
                    {
                        foreach(EventVolunteer ev in schoolEvent.Volunteers)
                        {
                            if (!string.IsNullOrEmpty(ev.Email))
                            {
                                volunteerEmails.Add(ev.Email);
                            }
                        }
                    }
                    schoolEvent.Volunteers.Clear();
                }
                db.Entry(schoolEvent).State = schoolEvent.SchoolEventId.Equals(0) ? EntityState.Unchanged : EntityState.Deleted;
                var ret = db.SaveChanges();

                //Send email notifying primary contact and all volunteers this event has been canceled
                if (notifyVolunteers)
                {
                    if ((volunteerEmails.Count>0) || string.IsNullOrEmpty(schoolEvent.PrimaryContactsEmail))
                    {
                        List<string> bcc = volunteerEmails;
                        List<string> to = new List<string>();
                        if(!string.IsNullOrEmpty(schoolEvent.PrimaryContactsEmail)) to.Add(schoolEvent.PrimaryContactsEmail);
                        string subject = "Canceled: \"" + schoolEvent.Title + "\" (starts on " + string.Format("{0:g}", schoolEvent.Start) + ")";
                        string body = "The \"" + schoolEvent.Title + "\" event has been canceled. <br>" +
                            (string.IsNullOrEmpty(customMessage) ? "" : customMessage) + "<br>" +
                            "<b>Start</b>: " + string.Format("{0:f}", schoolEvent.Start) + "<br>" +
                            "<b>End</b>: " + string.Format("{0:f}", schoolEvent.End) + "<br>" +
                            (!string.IsNullOrEmpty(schoolEvent.Location) ? ("<b>Location</b>: " + schoolEvent.Location + "<br>") : "") +
                            (!string.IsNullOrEmpty(schoolEvent.Description) ? ("<b>Description</b>: " + schoolEvent.Description + "<br>") : "");
                        bool emailsent = SendEventEmail(to, null, bcc, subject, body);
                    }
                }

                return ret;
            }
        }
        private static bool SendEventEmail(List<string> to, List<string> cc, List<string> bcc, string subject, string body)
        {
            bool ret = false;//email sent successfully?

            // Send email functionality goes here!
            var email = new Utilities.EmailMessage();
            email.SmtpAddress = Repository.ConfigurationItemRepository.GetValue(ModuleName, "SmtpServer");
            email.UserName = Repository.ConfigurationItemRepository.GetValue(ModuleName, "SmtpLoginUser");
            email.Password = Repository.ConfigurationItemRepository.GetValue(ModuleName, "SmtpLoginPassword");
            email.FromAddress = Repository.ConfigurationItemRepository.GetValue(ModuleName, "SendFromEmailAddress");
            email.ToAddress = to;
            email.Subject = subject;
            email.Body = body;
            email.BccAddress = bcc;
            email.CcAddress = cc;
            email.IsBodyHtml = true;

            ret = email.Send();
            return ret;
        }
    }
}
