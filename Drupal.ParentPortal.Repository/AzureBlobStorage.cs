using Drupal.ParentPortal.Models;
using Drupal.ParentPortal.Repository.Utilities;
using Microsoft.SharePoint.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Drupal.ParentPortal.Repository
{
    public class AzureBlobStorage
    {
        public static string _DefaultAzureContainer = "manual";
        public static string _SharePointDocLibPrefix = "spdoclib";

        #region Public Methods
        /// <summary>
        /// Uploads a document that was uploaded via a web form
        /// </summary>
        /// <param name="file"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public async Task<AzureFile> UploadDocumentAsync(HttpPostedFileBase file, Models.Document document)
        {
            string fileFullPath = null;
            string blobContainerName = _DefaultAzureContainer;

            if (file == null || file.ContentLength == 0)
            {
                return null;
            }

            try
            {
                //CloudStorageAccount cloudStorageAccount = AzureConnection.Get();
                //CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                //CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(blobContainerName);

                //if (await cloudBlobContainer.CreateIfNotExistsAsync())
                //{
                //    await cloudBlobContainer.SetPermissionsAsync(
                //        new BlobContainerPermissions
                //        {
                //            PublicAccess = BlobContainerPublicAccessType.Off
                //        }
                //    );
                //}

                string fileName = Path.GetFileName(file.FileName); 

                //CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
                //cloudBlockBlob.Properties.ContentType = file.ContentType;
                //await cloudBlockBlob.UploadFromStreamAsync(file.InputStream);

                //fileFullPath = cloudBlockBlob.Uri.ToString();

                fileFullPath = await SaveAzureDocumentBlob(file.ContentType, fileName, blobContainerName, file.InputStream);
            }
            catch (Exception ex)
            {
                // TODO: handle this exception and bubble up to the interface??
                return new AzureFile(document, false, ex.Message);
            }

            document.Url = fileFullPath;
            document.Uploaded = DateTime.UtcNow;

            return new AzureFile(document, true, null);
        }

        /// <summary>
        /// Upload document from a document library
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="document"></param>
        /// <param name="SPDocLibrary"></param>
        /// <param name="fileName"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public async Task<AzureFile> UploadDocumentAsync(Stream fileStream, Models.Document document, Models.DocumentLibrary SPDocLibrary, string fileName, string contentType)
        {
            string fileFullPath = null;
            string blobContainerName = _DefaultAzureContainer;

            try
            {
                // NOTE: Azure Blob container names must be all lowercase and cannot start with numbers
                if (SPDocLibrary != null && SPDocLibrary.DocumentLibraryId != 0)
                {
                    blobContainerName = _SharePointDocLibPrefix + SPDocLibrary.DocumentLibraryId.ToString();
                }

                fileFullPath = await SaveAzureDocumentBlob(contentType, fileName, blobContainerName, fileStream);
            }
            catch (Exception ex)
            {
                // TODO: handle this exception and bubble up to the interface??
                return new AzureFile(document, false, ex.Message);
            }

            document.Url = fileFullPath;
            document.Uploaded = DateTime.UtcNow;

            return new AzureFile(document, true, null); 
        }

        public async Task<string> DeleteDocumentLibraryDocumentsAsync(string documentLibraryId, bool alsoDeleteDocLib)
        {
            // delete the container and all files underneath it for the specified document library
            string blobContainerName = _SharePointDocLibPrefix;

            try
            {
                // NOTE: Azure Blob container names must be all lowercase and cannot start with numbers
                if (documentLibraryId != null)
                {
                    blobContainerName += documentLibraryId;
                }

                CloudStorageAccount cloudStorageAccount = AzureConnection.Get();
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(blobContainerName);

                if (alsoDeleteDocLib == true)
                {
                    // Deleting the container will also delete all underlying files in container
                    await cloudBlobContainer.DeleteIfExistsAsync();
                }
                else
                {
                    // Delete all underlying files in container
                    foreach (IListBlobItem item in cloudBlobContainer.ListBlobs(null, false))
                    {
                        if (item.GetType() == typeof(CloudBlockBlob))
                        {
                            CloudBlockBlob blob = (CloudBlockBlob)item;
                            await blob.DeleteIfExistsAsync();
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                // TODO: log this exception and bubble up to the interface??
                return ex.Message;
            }

        }

        public async Task DeleteSingleFileAsync(Document document)
        {
            try
            {
                StorageCredentials storageCredentials = new StorageCredentials(AzureConnection.StorageAccountName, AzureConnection.StorageAccountAccessKey);
                CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);

                Uri blobUri = new Uri(document.Url);
                CloudBlockBlob blob = new CloudBlockBlob(blobUri, storageCredentials);

                await blob.DeleteIfExistsAsync();
            }catch(Exception ex)
            {
                return;
            }
        }

        public async Task<MemoryStream> DownloadSingleFileAsync(Document document)
        {
            try
            {
                StorageCredentials storageCredentials = new StorageCredentials(AzureConnection.StorageAccountName, AzureConnection.StorageAccountAccessKey);
                CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);

                Uri blobUri = new Uri(document.Url);
                CloudBlockBlob blob = new CloudBlockBlob(blobUri, storageCredentials);

                MemoryStream memStream = new MemoryStream();
                await blob.DownloadToStreamAsync(memStream);

                return memStream;
            }catch(Exception ex)
            {
                return null;
            }
        }
        #endregion


        #region Private Helper Methods
        private async Task<string> SaveAzureDocumentBlob(string contentType, string fileName, string blobContainerName, Stream stream)
        {
            try
            {
                string fileFullPath = null;
                CloudStorageAccount cloudStorageAccount = AzureConnection.Get();
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(blobContainerName);

                await cloudBlobContainer.CreateIfNotExistsAsync();

                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
                //StorageCredentials creds = new StorageCredentials(AzureConnection.StorageAccountName, AzureConnection.StorageAccountAccessKey);
                //CloudBlockBlob cloudBlockBlob = new CloudBlockBlob(fileName, creds);
                cloudBlockBlob.Properties.ContentType = contentType;
                await cloudBlockBlob.UploadFromStreamAsync(stream);

                fileFullPath = cloudBlockBlob.Uri.ToString();

                return fileFullPath;
            }catch(Exception ex)
            {
                return null;
            }
        }

       

        #endregion
    }
}
