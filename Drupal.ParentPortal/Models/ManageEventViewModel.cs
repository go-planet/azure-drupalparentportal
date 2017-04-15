using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Drupal.ParentPortal.Models
{
    public class ManageEventViewModel
    {
        public SchoolEvent SchoolEvent { get; set; }

        public ConfigurationItem SmtpServer { get; set; }
        public ConfigurationItem SendFromEmailAddress { get; set; }
        public ConfigurationItem SmtpLoginUser { get; set; }
        public ConfigurationItem SmtpLoginPassword { get; set; }
        [DefaultValue(false)]
        public bool NotifyVolunteersUpdate { get; set; }

        public string CustomMessageUpdate { get; set; }
        public bool NotifyVolunteersDelete { get; set; }

        public string CustomMessageDelete { get; set; }
        public List<System.Web.Mvc.SelectListItem> Audiences { set; get; }
    }
}