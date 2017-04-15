using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drupal.ParentPortal.Models
{
    public class ConfigurationItem
    {
        [Key]
        public int ConfigurationItemId { get; set; }
        public string Module { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public bool Required { get; set; }

    }
}
