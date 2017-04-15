namespace Drupal.ParentPortal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    public class AlertFeed : IAudience
    {
        [Display(Name = "Alert Management Id")]
        [Key]
        public long AlertManagementId { get; set; }

        [Display(Name = "Alert Uri")]
        public int AlertUri { get; set; }
        
        [Display(Name = "Label Field")]
        public String LabelField { get; set; }

        [Display(Name = "Link Field")]
        public String LinkField { get; set; }

        [Display(Name = "Audience Field")]
        public String AudienceField { get; set; }

        public long? AudienceId { get; set; }
        public Audience Audience { get; set; }
    }
}
