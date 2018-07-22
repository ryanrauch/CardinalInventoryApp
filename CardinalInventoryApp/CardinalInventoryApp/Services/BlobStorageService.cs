using CardinalInventoryApp.Services.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardinalInventoryApp.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly CloudStorageAccount _cloudStorageAccount;
        private readonly CloudBlobClient _blobClient;

        public BlobStorageService()
        {
            _cloudStorageAccount = CloudStorageAccount.Parse(Secrets.CardinalBlobStorageKey);
            _blobClient = _cloudStorageAccount.CreateCloudBlobClient();
        }

        public async Task<List<T>> GetBlobs<T>(string containerName, string prefix = "", int? maxresultsPerQuery = null, BlobListingDetails blobListingDetails = BlobListingDetails.None) where T : ICloudBlob
        {
            var blobContainer = _blobClient.GetContainerReference(containerName);

            var blobList = new List<T>();
            BlobContinuationToken continuationToken = null;

            //try
            //{
                do
                {
                    var response = await blobContainer.ListBlobsSegmentedAsync(prefix, true, blobListingDetails, maxresultsPerQuery, continuationToken, null, null);

                    continuationToken = response?.ContinuationToken;

                    foreach (var blob in response?.Results?.OfType<T>())
                    {
                        blobList.Add(blob);
                    }

                } while (continuationToken != null);
            //}
            //catch (Exception e)
            //{
            //    //Handle Exception
            //}

            return blobList;
        }

        public async Task<CloudBlockBlob> SaveBlockBlob(string containerName, byte[] blob, string blobTitle)
        {
            var blobContainer = _blobClient.GetContainerReference(containerName);

            var blockBlob = blobContainer.GetBlockBlobReference(blobTitle);
            await blockBlob.UploadFromByteArrayAsync(blob, 0, blob.Length);

            return blockBlob;
        }
    }
}
