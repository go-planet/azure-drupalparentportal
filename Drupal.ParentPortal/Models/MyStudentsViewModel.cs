using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Drupal.ParentPortal.Models
{
    public class MyStudentsViewModel
    {
        public List<Student> MyStudents { get; set; }

        public Student NewStudent { get; set; }
        public string LoggedInUser { get; set; }//remove once integrated with Drupal

        public string ClientId { get; set; }//remove once integrated with Drupal

        public string Secret { get; set; }//remove once integrated with Drupal
        public IEnumerable<System.Web.Mvc.SelectListItem> Grades { set; get; }
    }
}