using Drupal.ParentPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drupal.ParentPortal.Repository.Utilities
{
    public class AzureFile
    {
        #region Properties
        public Document Document { set; get; }
        public bool UploadToAzureSuccessful { set; get; }
        
        public string UploadToAzureErrorMessage { set; get; }
        #endregion

        #region Constructor
        public AzureFile(Document doc, bool uploadToAzureSuccessful, string uploadToAzureErrorMessage)
        {
            Document = doc;
            UploadToAzureSuccessful = uploadToAzureSuccessful;
            UploadToAzureErrorMessage = uploadToAzureErrorMessage;
        }
        #endregion
    }
}
