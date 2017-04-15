namespace Drupal.ParentPortal.Repository
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class AppManagement
    {
        public static Models.AppManagement Get(long id)
        {
            using (var db = new Context.SqlContext())
            {                
                return db.AppManagement.FirstOrDefault<Models.AppManagement>(v => v.AppManagementId.Equals(id));
            }
        }

        public static Models.AppManagement Get()
        {
            using (var db = new Context.SqlContext())
            {
                return db.AppManagement.FirstOrDefault<Models.AppManagement>();
            }
        }

        public static Int32 Save(Models.AppManagement appManagement)
        {
            using (var db = new Context.SqlContext())
            {
                db.Entry(appManagement).State = appManagement.AppManagementId.Equals(0) ? EntityState.Added : EntityState.Modified;

                return db.SaveChanges();
            }
        }
        /// <summary>
        /// Validates that the supplied clientid and secret match the corresponding configuration settings. Always returns true if in debug mode.
        /// </summary>
        /// <param name="clientid"></param>
        /// <param name="secret"></param>
        /// <returns>True if the supplied clientid and secret match the configuration settings, otherwise false</returns>
        public static bool IsValidClientSecret(string clientid, string secret)
        {

            try
            {
                if ((string.IsNullOrEmpty(clientid)) || (string.IsNullOrEmpty(secret))) return false;
                Guid ci = Guid.Parse(clientid);
                var am = Get();
                return ((ci == am.ClientId) && (secret == am.Secret)) ? true : false;
            }catch(Exception ex)
            {
                return false;
            }

        }
        /// <summary>
        /// Only validates that the userid is not null or empty. Always returns true if in debug mode.
        /// </summary>
        /// <param name="userid"></param>
        /// <returns>True if userid is not null or empty, otherwise returns false</returns>
        public static bool IsValidUser(string userid)
        {

            try
            {
                if ((string.IsNullOrEmpty(userid)) || (userid == "0")) return false;
                else return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}