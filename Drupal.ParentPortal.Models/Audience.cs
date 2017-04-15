namespace Drupal.ParentPortal.Models
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class Audience
    {

        [Key]
        public long AudienceId { get; set; }

        [Required(ErrorMessage = "Audience Name is required.")]
        [Display(Name = "Audience")]
        public string AudienceName { get; set; }
        
        [Display(Name = "School Audience Value")]
        public string SchoolAudienceValue { get; set; }
        
        [Display(Name = "Grade Level Audience Value")]
        public string GradeLevelAudienceValue{ get; set; }

        [Display(Name = "Teacher Audience Value")]
        public string TeacherAudienceValue { get; set; }

        [Display(Name = "Selectable")]
        [DefaultValue(true)]
        public bool Selectable { get; set; }
    }
}
