using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Drupal.ParentPortal.Models
{
    public class DocumentsViewModel
    {
        public DocumentLibrary NewSPDocumentLibrary { set; get; }

        public DocumentLibrary EditSPDocumentLibrary { set; get; }

        public Document NewDocument { set; get; }

        public Document EditDocument { set; get; }

        public List<Document> Documents { set; get; }

        public List<DocumentLibrary> SPDocumentLibraries { set; get; }

        public List<Audience> Audiences { set; get; }

        public bool? DocumentUploadSucceeded { set; get; }

        public string DocumentUploadFailedMsg { set; get; }

        public DocumentSettingsViewModel Settings { set; get; }
    }
}