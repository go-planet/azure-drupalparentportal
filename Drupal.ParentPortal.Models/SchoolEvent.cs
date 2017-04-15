namespace Drupal.ParentPortal.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class SchoolEvent : IAudience
    {
        [JsonProperty(PropertyName = "schooleventid")]
        [Display(Name = "School Event Id")]
        [Key]
        public long SchoolEventId { get; set; }

        [JsonProperty(PropertyName = "title")]
        [Required(ErrorMessage = "A title is required.")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:g}", ApplyFormatInEditMode = true)]
        [JsonProperty(PropertyName = "start")]
        [Required(ErrorMessage = "A start date and time is required.")]
        [Display(Name = "Start")]
        public DateTime? Start { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:g}", ApplyFormatInEditMode = true)]
        [JsonProperty(PropertyName = "end")]
        [Required(ErrorMessage = "An end date and time is required.")]
        [Display(Name = "End")]
        public DateTime? End { get; set; }

        [JsonProperty(PropertyName = "description")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "location")]
        [Display(Name = "Location")]
        public string Location { get; set; }
        
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        [JsonProperty(PropertyName = "primarycontactsemail")]
        [Display(Name = "Primary Contact's Email")]
        public string PrimaryContactsEmail { get; set; }

        [JsonProperty(PropertyName = "isvolunteeropportunity")]
        [Display(Name = "Is Volunteer Opportunity")]
        [DefaultValue(false)]
        public bool IsVolunteerOpportunity { get; set; }

        [JsonProperty(PropertyName = "maxvolunteers")]
        [Display(Name = "Max Volunteers")]
        [DefaultValue(0)]
        public int MaxVolunteers { get; set; } // zero means no limit to number of volunteers the system will allow to volunteer

        [JsonProperty(PropertyName = "registeredvolunteers")]
        [Display(Name = "RegisteredVolunteers")]
        [DefaultValue(0)]
        public int RegisteredVolunteers { get; set; }

        [JsonProperty(PropertyName = "volunteers")]
        [Display(Name = "Volunteers")]
        public virtual ICollection<EventVolunteer> Volunteers { get; set; }

        
        [DataType(DataType.Url)]
        [JsonProperty(PropertyName = "actionlink")]
        [Display(Name = "Action Link")]
        [Url]
        public string ActionLink { get; set; }

        [JsonProperty(PropertyName = "actionbuttontext")]
        [Display(Name = "Action Button Text")]
        public string ActionButtonText { get; set; }

        public long? AudienceId { get; set; }
        public virtual Audience Audience { get; set; }
    }

}