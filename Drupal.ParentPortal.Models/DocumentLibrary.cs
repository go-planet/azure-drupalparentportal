namespace Drupal.ParentPortal.Models
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;


    public class DocumentLibrary : IAudience
    {
        [Key]
        public long DocumentLibraryId { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Library Name is required.")]
        [Display(Name = "Library Name")]
        public string LibraryName { get; set; }

        [Required(ErrorMessage = "Site Url is required.")]
        [Display(Name = "Site Url")]
        [Url]
        public string SiteUrl { get; set; }
        
        [Display(Name = "AudienceId")]
        public long? AudienceId { get; set; }

        [Display(Name = "Audience")]
        public Audience Audience { get; set; }
    }
}
