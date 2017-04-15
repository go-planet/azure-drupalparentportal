namespace Drupal.ParentPortal.Models
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    public class Student
    {
        [Key]
        public long StudentId { get; set; }

        [Required(ErrorMessage = "Parent User Id is required.")]
        [Display(Name = "Parent User Id")]
        public string ParentUserId { get; set; }


        [Required(ErrorMessage = "Student's School Id is required.")]
        [Display(Name = "Student Id")]
        public string StudentsSchoolId { get; set; }
        
        [Required(ErrorMessage = "First name is required.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        //[Required(ErrorMessage = "School is required.")]
        [Display(Name = "School")]
        public string School { get; set; }


        //[Required(ErrorMessage = "Teacher is required.")]
        [Display(Name = "Teacher")]
        public string Teacher { get; set; }

        //[Required(ErrorMessage = "Grade Level is required.")]
        [Display(Name = "Grade Level")]
        public string GradeLevel { get; set; }

        [Required(ErrorMessage = "Date of Birth is required.")]
        [Display(Name = "Date of Birth")]
        public DateTime? DoB { get; set; }
        
        [Display(Name = "Is Validated")]
        [DefaultValue(false)]
        public bool IsValidated { get; set; }

    }
}
