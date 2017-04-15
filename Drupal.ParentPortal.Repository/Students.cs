namespace Drupal.ParentPortal.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Data.Entity;
    using System.Text;
    using System.Threading.Tasks;
    using Drupal.ParentPortal.Models;

    public class Students
    {
        public const string ModuleName = "MyStudents";
        public static Student GetById(long studentId)
        {
            using (var db = new Context.SqlContext())
            {
                var sList = db.Student.Where(s => s.StudentId == studentId).FirstOrDefault();
                return sList;
            }
        }
        public static List<Models.Student> GetByParent(string parentUserId)
        {
            using (var db = new Context.SqlContext())
            {
                var sList = db.Student.Where(s => s.ParentUserId == parentUserId).ToList();
                return sList;
            }
        }

        public static int SaveStudent(Models.Student student)
        {
            //Todo: validate student against web service here!
            try
            {
                using (var db = new Context.SqlContext())
                {

                    db.Entry(student).State = student.StudentId.Equals(0) ? EntityState.Added : EntityState.Modified;
                    if (db.Entry(student).State == EntityState.Added)
                    {
                        var ret = db.SaveChanges();
                        var auds = Repository.Audience.CreateMissingAudiencesFromNewStudent(student);
                        return ret;
                    }
                    else
                    {
                        return db.SaveChanges();
                    }
                }
            }catch(Exception ex)
            {
                return -1;
            }
        }
        public static int RemoveStudent(Student student)
        {
            int ret = -1;
            using (var db = new Context.SqlContext())
            {
                db.Entry(student).State = student.StudentId.Equals(0) ? EntityState.Unchanged : EntityState.Deleted;
                ret = db.SaveChanges();
            }
            return ret;
        }
        public static int RemoveStudent(long studentId)
        {
            var model = GetById(studentId);
            Int32 result = -1;
            if (model != null)
            {
                result = RemoveStudent(model);
            }
            return result;
        }

    }
}
