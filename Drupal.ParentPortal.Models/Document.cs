namespace Drupal.ParentPortal.Models
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class Document : IAudience
    {
        [Key]
        public long DocumentId { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage ="A document name is required.")]
        public string Name { get; set; }

        [Display(Name = "Url")]
        //[Url]
        [Required(ErrorMessage = "A Url for the document is required.")]
        public string Url { get; set; }

        [Display(Name = "Extension")]
        [Required(ErrorMessage = "A file extension is required.")]
        public string Extension { get; set; }
        
        [Display(Name = "Uploaded")]        
        public DateTime? Uploaded { get; set; }

        [Display(Name = "AudienceId")]
        public long? AudienceId { get; set; }

        [Display(Name = "Audience")]
        public virtual Audience Audience { get; set; }

        public long? DocumentLibraryId { get; set; }
        public virtual DocumentLibrary DocumentLibrary { get; set; }
    }
}
