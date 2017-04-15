namespace Drupal.ParentPortal.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class AppManagement
    {
        [Key]
        public long AppManagementId { get; set; }


        [Required(ErrorMessage = "A Guid pointer is required.")]
        [Display(Name ="App Management Id")]
        public Guid ClientId { get; set; }

        [Required(ErrorMessage = "Secret is required.")]
        [Display(Name ="Client Secret")]
        public String Secret { get; set; }

        //[Required(ErrorMessage = "The location of your drupal site is required.")]
        //[Display(Name ="Drupal Url")]
        //public String RemoteUri { get; set; }
    }
}
