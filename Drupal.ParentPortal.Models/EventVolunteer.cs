namespace Drupal.ParentPortal.Models
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class EventVolunteer
    {
        [Key]
        public long EventVolunteerId { get; set; }

        public long SchoolEventId { get; set; }
        public virtual SchoolEvent SchoolEvent { get; set; }

        [Required(ErrorMessage = "User Id is required.")]
        [Display(Name = "User Id")]
        public string UserId{ get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [Display(Name ="First Name")]        
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone is required.")]
        [Display(Name = "Phone")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        
        [Display(Name = "Street Address")]
        public string StreetAddress { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "State")]
        public string State { get; set; }

        [Display(Name = "ZIP Code")]
        public string ZIPCode { get; set; }

        [Display(Name = "RegisteredDate")]
        public DateTime? RegisteredDate { get; set; }

        //[Required(ErrorMessage = "A volunteer status is required.")]
        [Display(Name = "Status")]
        public string Status { get; set; }
    }
}
