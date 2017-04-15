

namespace Drupal.ParentPortal.Models
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    public class Alert : IAudience
    {
        [Key]
        public long AlertId { get; set; }

        [Required(ErrorMessage = "A title is required.")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Url")]
        [Url]
        [Required(ErrorMessage = "A Url for the document is required.")]
        public string Url { get; set; }

        public long? AudienceId { get; set; }
        public Audience Audience { get; set; }
    }
}
