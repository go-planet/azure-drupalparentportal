using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drupal.ParentPortal.Repository
{
    public class ConfigurationItemRepository
    {
        public static Models.ConfigurationItem Get(long id)
        {
            using (var db = new Context.SqlContext())
            {
                if (id == 0) return new Models.ConfigurationItem();
                return db.ConfigurationItem.FirstOrDefault<Models.ConfigurationItem>(v => v.ConfigurationItemId.Equals(id));
            }
        }

        public static IEnumerable<Models.ConfigurationItem> GetAll()
        {
            try
            {
                using (var db = new Context.SqlContext())
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    return db.ConfigurationItem.ToList();
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return null;
            }
        }
        public static IEnumerable<Models.ConfigurationItem> GetAll(string ModuleName)
        {
            try
            {
                using (var db = new Context.SqlContext())
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    return db.ConfigurationItem.Where(i => i.Module == ModuleName).ToList();
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return null;
            }
        }
        public static string GetValue(string ModuleName,string Key)
        {
            try
            {
                using (var db = new Context.SqlContext())
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    Models.ConfigurationItem ci = db.ConfigurationItem.Where(i => i.Module == ModuleName && i.Key == Key).FirstOrDefault();
                    if((ci != null) && (ci.Value != null))
                    {
                        return ci.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return "";
            }
            return "";
        }

        /// <summary>
        /// Returns the first ConfigurationItem that matches the supplied ModuleName and Key. 
        /// If one does not exist, then a new one is returned that will need to be saved back to the database to get created.
        /// </summary>
        /// <param name="ModuleName"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static Models.ConfigurationItem GetUniqueItem(string ModuleName, string Key)
        {
            try
            {
                using (var db = new Context.SqlContext())
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    Models.ConfigurationItem ci = db.ConfigurationItem.Where(i => i.Module == ModuleName && i.Key == Key).FirstOrDefault();
                    if (ci != null)
                    {
                        return ci;
                    }else
                    {
                        ci = new Models.ConfigurationItem();
                        ci.Module = ModuleName;
                        ci.Key = Key;
                        return ci;
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return null;
            }
        }

        public static int Save(Models.ConfigurationItem configurationItem)
        {

            using (var db = new Context.SqlContext())
            {
                db.Entry(configurationItem).State = configurationItem.ConfigurationItemId.Equals(0) ? EntityState.Added : EntityState.Modified;
                return db.SaveChanges();
            }
        }
    }
}
