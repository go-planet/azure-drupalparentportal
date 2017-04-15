using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drupal.ParentPortal.Repository
{
    public class DocumentLibrary
    {
        #region Public Static Methods
        public static Models.DocumentLibrary Get(long id)
        {
            using (var db = new Context.SqlContext())
            {
                if (id == 0) return new Models.DocumentLibrary();

                db.Configuration.LazyLoadingEnabled = false;
                return db.DocumentLibrary.Include(d => d.Audience).FirstOrDefault<Models.DocumentLibrary>(v => v.DocumentLibraryId.Equals(id));
            }
        }

        public static List<Models.DocumentLibrary> GetAll()
        {
            try
            {
                using (var db = new Context.SqlContext())
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    var libs = db.DocumentLibrary.Include(d => d.Audience).ToList();
                    return libs;
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return null;
            }
        }

        public static long Save(Models.DocumentLibrary library)
        {

            using (var db = new Context.SqlContext())
            {
                db.Entry(library).State = library.DocumentLibraryId.Equals(0) ? EntityState.Added : EntityState.Modified;
                var ret = db.SaveChanges();

                return library.DocumentLibraryId;
            }
        }

        public static int Delete(Models.DocumentLibrary library)
        {
            using (var db = new Context.SqlContext())
            {
                db.Entry(library).State = library.DocumentLibraryId.Equals(0) ? EntityState.Unchanged : EntityState.Deleted;
                var ret = db.SaveChanges();

                return ret;
            }
        }
        #endregion
    }
}
