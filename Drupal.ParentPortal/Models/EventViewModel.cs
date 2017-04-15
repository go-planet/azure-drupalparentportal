using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Drupal.ParentPortal.Models
{
    public class EventViewModel
    {
        //public List<SchoolEvent> SchoolEvents { get; set; }
        public EventVolunteer VolunteerInfo { get; set; }

        public List<EventVolunteer> UpcomingVolunteerEvents { get; set; }

        //public string LoggedInUser { get; set; }

        //public string ClientId { get; set; }//remove once integrated with Drupal

        //public string Secret { get; set; }//remove once integrated with Drupal
    }
}