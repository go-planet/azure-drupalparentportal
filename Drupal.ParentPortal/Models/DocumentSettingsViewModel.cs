using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Drupal.ParentPortal.Models
{
    public class DocumentSettingsViewModel
    {
        public ConfigurationItem ConnectionString { get; set; }
        public ConfigurationItem AccountName { get; set; }
        public ConfigurationItem AccessKey { get; set; }
    }
}