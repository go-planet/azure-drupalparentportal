using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drupal.ParentPortal.Repository.Utilities
{
    public class AzureConnection
    {
        private const string _moduleName = Repository.Documents.ModuleName;
        public static CloudStorageAccount Get()
        {
            return CloudStorageAccount.Parse(Repository.ConfigurationItemRepository.GetValue(_moduleName, "ConnectionString"));
        }

        public static string StorageAccountName
        {
            get { return Repository.ConfigurationItemRepository.GetValue(_moduleName, "AccountName"); }
        }

        public static string StorageAccountAccessKey
        {
            get { return Repository.ConfigurationItemRepository.GetValue(_moduleName, "AccessKey"); }
        }
    }
}
