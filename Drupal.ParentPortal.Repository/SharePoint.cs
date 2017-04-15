using Drupal.ParentPortal.Repository.Utilities;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Drupal.ParentPortal.Models;

namespace Drupal.ParentPortal.Repository
{
    public class SharePoint
    {
        public async Task<List<Models.Document>> DownloadFilesToAzure(Models.DocumentLibrary docLib)
        {
            List<Models.Document> documentsSaved = new List<Models.Document>();

            try
            {
                using (ClientContext context = new ClientContext(docLib.SiteUrl))
                {
                    ListItemCollection items = GetFilesRecursively(context, docLib);

                    // For each file returned, download the file
                    foreach (var item in items)
                    {
                        // Pull file information from SharePoint
                        string fileRef = (string)item["FileRef"];
                        var fileName = System.IO.Path.GetFileName(fileRef);
                        var fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(context, fileRef);

                        // Construct the document object to save to the db
                        Models.Document newDocument = new Models.Document();
                        newDocument.Name = fileName;
                        newDocument.Extension = Path.GetExtension(fileName);
                        newDocument.DocumentLibraryId = docLib.DocumentLibraryId;
                        newDocument.AudienceId = docLib.AudienceId;

                        //Console.WriteLine("File name => " + fileName);
                        //Console.WriteLine("Mime Type => " + MIMEAssistant.GetMIMEType(fileName));
                        //ClientResult<Stream> fileStream = file.OpenBinaryStream();
                        //Microsoft.SharePoint.Client.File.OpenBinaryDirect(context, fileRef);

                        // Save to Azure 
                        AzureBlobStorage azureStorage = new AzureBlobStorage();
                        AzureFile azureFile = await azureStorage.UploadDocumentAsync(fileInfo.Stream, newDocument, docLib, fileName, MIMEAssistant.GetMIMEType(fileName));

                        // Set url to document object to save to db
                        newDocument.Url = azureFile.Document.Url;
                        newDocument.Uploaded = DateTime.UtcNow;

                        documentsSaved.Add(newDocument);
                    }
                }

                return documentsSaved;
            }
            catch (Exception e)
            {
                // TODO: add error handling here...
                Console.WriteLine("SharePoint.DownloadFilesToAzure debug, " + e.Message);
                return null;
            }
        }

        private ListItemCollection GetFilesRecursively(ClientContext context, Models.DocumentLibrary docLib)
        {
            SecureString secureString = new SecureString();
            docLib.Password.ToList().ForEach(secureString.AppendChar);

            context.Credentials = new SharePointOnlineCredentials(docLib.Username, secureString);

            // Get all files from the current document library
            var qry = new CamlQuery();
            qry.ViewXml = "<View Scope='RecursiveAll'>" +
                                        "<Query>" +
                                            "<Where>" +
                                                "<Eq>" +
                                                    "<FieldRef Name='FSObjType' />" +
                                                    "<Value Type='Integer'>0</Value>" +
                                                "</Eq>" +
                                        "</Where>" +
                                        "</Query>" +
                                    "</View>";

            var sourceList = context.Web.Lists.GetByTitle(docLib.LibraryName);
            ListItemCollection items = sourceList.GetItems(qry);
            context.Load(items);
            context.ExecuteQuery();

            return items;
        }
    }
}
