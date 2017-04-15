namespace Drupal.ParentPortal.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    public interface IAudience
    {
        long? AudienceId { get; set; }
        Audience Audience { get; set; }
    }
}
