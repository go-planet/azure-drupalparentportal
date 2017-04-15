using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drupal.ParentPortal.Repository
{
    public class Documents
    {
        #region Properties
        public const string ModuleName = "Documents";
        #endregion

        #region Public Static Methods
        public static Models.Document Get(long id)
        {
            using (var db = new Context.SqlContext())
            {
                if (id == 0) return new Models.Document();

                db.Configuration.LazyLoadingEnabled = false;
                return db.Document.Include(x => x.Audience).Include(x => x.DocumentLibrary).FirstOrDefault<Models.Document>(v => v.DocumentId.Equals(id));
            }
        }

        public static List<Models.Document> GetAllFromDocumentLibrary(long documentLibraryId)
        {
            using (var db = new Context.SqlContext())
            {
                if (documentLibraryId == 0) return new List<Models.Document>();

                db.Configuration.LazyLoadingEnabled = false;
                var results = db.Document.Include(x => x.Audience).Include(x => x.DocumentLibrary).Where(v => v.DocumentLibraryId.HasValue.Equals(true) && v.DocumentLibraryId.Value.Equals(documentLibraryId)).ToList<Models.Document>();

                return results;
            }
        }

        public static List<Models.Document> GetAll()
        {
            try
            {
                using (var db = new Context.SqlContext())
                {
                    /// TODO: Add audience option for filtering here
                    db.Configuration.LazyLoadingEnabled = false;
                    var ses = db.Document.Include(x => x.Audience).Include(x => x.DocumentLibrary).ToList();

                    return ses;
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return null;
            }
        }

        public static List<Models.Document> GetAll(string userId)
        {
            try
            {
                var audiences = Repository.Audience.GetUsersAudienceIds(userId);
                using (var db = new Context.SqlContext())
                {
                    /// TODO: Add audience option for filtering here
                    db.Configuration.LazyLoadingEnabled = false;
                    var ses = db.Document.Include(x => x.Audience).Include(x => x.DocumentLibrary).Where(i => i.AudienceId == null || audiences.Contains(i.AudienceId)).ToList();

                    return ses;
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return null;
            }
        }

        public static List<Models.Document> GetByPage(string userId, int page)
        {
            try
            {
                var audiences = Repository.Audience.GetUsersAudienceIds(userId);
                using (var db = new Context.SqlContext())
                {
                    /// TODO: Add audience option for filtering here
                    db.Configuration.LazyLoadingEnabled = false;
                    var ses = db.Document.Include(x => x.Audience).Include(x => x.DocumentLibrary).Where(i => i.AudienceId == null || audiences.Contains(i.AudienceId)).ToList().OrderBy(x => x.Name).Skip(10 * page).Take(10);
                    // TODO: Optimize Query

                    return ses.ToList();
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return null;
            }
        }

        public static int GetCount(string userId)
        {
            try
            {
                var audiences = Repository.Audience.GetUsersAudienceIds(userId);
                using (var db = new Context.SqlContext())
                {
                    /// TODO: Add audience option for filtering here
                    db.Configuration.LazyLoadingEnabled = false;
                    var ses = db.Document.Include(x => x.Audience).Include(x => x.DocumentLibrary).Where(i => i.AudienceId == null || audiences.Contains(i.AudienceId)).Count();

                    return ses;
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return 0;
            }
        }

        public static long Save(Models.Document document)
        {
            try
            {
                using (var db = new Context.SqlContext())
                {
                    db.Entry(document).State = document.DocumentId.Equals(0) ? EntityState.Added : EntityState.Modified;
                    var ret = db.SaveChanges();

                    return document.DocumentId;
                }
            }catch(Exception ex)
            {
                return -1;
            }
        }

        public static object Delete(Models.Document document)
        {
            using (var db = new Context.SqlContext())
            {
                db.Entry(document).State = document.DocumentId.Equals(0) ? EntityState.Unchanged : EntityState.Deleted;
                var ret = db.SaveChanges();

                return ret;
            }
        }
        #endregion

    }
}
