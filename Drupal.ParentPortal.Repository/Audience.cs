using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Drupal.ParentPortal.Repository
{
    public class Audience
    {
        #region Public Static Methods
        public static Models.Audience Get(long id)
        {
            using (var db = new Context.SqlContext())
            {
                if (id == 0) return new Models.Audience();
                return db.Audience.FirstOrDefault<Models.Audience>(v => v.AudienceId.Equals(id));
            }
        }

        public static List<Models.Audience> GetAll()
        {
            try
            {
                using (var db = new Context.SqlContext())
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    var aud = db.Audience.OrderBy(a => a.AudienceName).ToList();
                    return aud;
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return null;
            }
        }
        public static List<System.Web.Mvc.SelectListItem> GetAudienceSelectItems()
        {
            var ret = Audience.GetAll().Where(i => i.Selectable == true).OrderBy(i => i.AudienceName).Select(a =>
            {
                return new SelectListItem()
                {
                    Text = a.AudienceName,
                    Value = a.AudienceId.ToString(),
                    Selected = false
                };
            }).ToList();
            ret.Insert(0, new SelectListItem { Text = "", Value = "" });
            return ret;
        }
        public static Models.Audience GetSchoolAudience(string school, bool createIfMissing)
        {
            using (var db = new Context.SqlContext())
            {
                if (string.IsNullOrEmpty(school))
                {
                    return null;
                }
                var n = db.Audience.FirstOrDefault<Models.Audience>(v => v.SchoolAudienceValue.Equals(school) & v.GradeLevelAudienceValue.Equals(null) & v.TeacherAudienceValue.Equals(null));
                if((n == null) && (createIfMissing))
                {
                    n = new Models.Audience();
                    n.SchoolAudienceValue = school;
                    n.AudienceName = school;
                    n.Selectable = true;
                    Save(n);
                }
                return n;
            }
        }
        public static Models.Audience GetSchoolGradeAudience(string school, string grade, bool createIfMissing)
        {
            using (var db = new Context.SqlContext())
            {
                if ((string.IsNullOrEmpty(school)) || (string.IsNullOrEmpty(grade)))
                {
                    return null;
                }
                var n = db.Audience.FirstOrDefault<Models.Audience>(v => v.SchoolAudienceValue.Equals(school) & v.GradeLevelAudienceValue.Equals(grade) & v.TeacherAudienceValue.Equals(null));
                if ((n == null) && (createIfMissing))
                {
                    n = new Models.Audience();
                    n.SchoolAudienceValue = school;
                    n.GradeLevelAudienceValue = grade;
                    n.AudienceName = school + " - " + grade;
                    n.Selectable = true;
                    Save(n);
                }
                return n;
            }
        }
        public static Models.Audience GetSchoolTeacherAudience(string school, string teacher, bool createIfMissing)
        {
            using (var db = new Context.SqlContext())
            {
                if ((string.IsNullOrEmpty(school)) || (string.IsNullOrEmpty(teacher)))
                {
                    return null;
                }
                var n = db.Audience.FirstOrDefault<Models.Audience>(v => v.SchoolAudienceValue.Equals(school) & v.GradeLevelAudienceValue.Equals(null) & v.TeacherAudienceValue.Equals(teacher));
                if ((n == null) && (createIfMissing))
                {
                    n = new Models.Audience();
                    n.SchoolAudienceValue = school;
                    n.TeacherAudienceValue = teacher;
                    n.AudienceName = school + " - " + teacher;
                    n.Selectable = true;
                    Save(n);
                }
                return n;
            }
        }
        public static Models.Audience GetGradeAudience(string grade, bool createIfMissing)
        {
            using (var db = new Context.SqlContext())
            {
                if (string.IsNullOrEmpty(grade))
                {
                    return null;
                }
                var n = db.Audience.FirstOrDefault<Models.Audience>(v => v.SchoolAudienceValue.Equals(null) & v.GradeLevelAudienceValue.Equals(grade) & v.TeacherAudienceValue.Equals(null));
                if ((n == null) && (createIfMissing))
                {
                    n = new Models.Audience();
                    n.GradeLevelAudienceValue = grade;
                    n.AudienceName = grade;
                    n.Selectable = true;
                    Save(n);
                }
                return n;
            }
        }

        public static int Save(Models.Audience audience)
        {

            using (var db = new Context.SqlContext())
            {
                db.Entry(audience).State = audience.AudienceId.Equals(0) ? EntityState.Added : EntityState.Modified;
                var ret = db.SaveChanges();

                return ret;
            }
        }

        public static object Delete(Models.Audience audience)
        {
            using (var db = new Context.SqlContext())
            {
                db.Entry(audience).State = audience.AudienceId.Equals(0) ? EntityState.Unchanged : EntityState.Deleted;
                var ret = db.SaveChanges();

                return ret;
            }
        }
        public static bool CreateMissingAudiencesFromNewStudent(Models.Student student)
        {
            var ret = false;
            try
            {
                var schoolAudience = GetSchoolAudience(student.School, true);
                var schoolGradeAudience = GetSchoolGradeAudience(student.School, student.GradeLevel, true);
                var schoolTeacherAudience = GetSchoolTeacherAudience(student.School, student.Teacher, true);
                var gradeAudience = GetGradeAudience(student.GradeLevel, true);
                ret = true;
            }
            catch (Exception ex)
            {
                ret = false;
            }
            return ret;
        }
        public static List<Models.Audience> GetAudiencesFromStudent(Models.Student student)
        {
            var error = false;
            List<Models.Audience> audiences = new List<Models.Audience>();
            try
            {
                var a1 = GetSchoolAudience(student.School, false);
                if(a1 != null) audiences.Add(a1);
                var a2 = GetSchoolGradeAudience(student.School, student.GradeLevel, false);
                if (a2 != null) audiences.Add(a2);
                var a3 = GetSchoolTeacherAudience(student.School, student.Teacher, false);
                if (a3 != null) audiences.Add(a3);
                var a4 = GetGradeAudience(student.GradeLevel, false);
                if (a4 != null) audiences.Add(a4);
                error = true;
            }
            catch (Exception ex)
            {
                error = false;
            }
            return audiences;
        }

        public static List<Models.Audience> GetUsersAudiences(string userId)
        {
            var students = Repository.Students.GetByParent(userId);

            List<Models.Audience> audiences = new List<Models.Audience>();
            foreach(Models.Student stu in students)
            {
                audiences.AddRange(GetAudiencesFromStudent(stu));
            }
            return audiences;
        }
        public static List<long?> GetUsersAudienceIds(string userid)
        {
            var aus = GetUsersAudiences(userid);
            return aus.Select(i => (long?)i.AudienceId).ToList();
        }
        #endregion
    }
}
