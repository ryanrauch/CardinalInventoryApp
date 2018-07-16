using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace CardinalInventoryApp.Services.Interfaces
{
    public interface IBlobStorageService
    {
        Task<List<T>> GetBlobs<T>(string containerName, string prefix = "", int? maxresultsPerQuery = null, BlobListingDetails blobListingDetails = BlobListingDetails.None) where T : ICloudBlob;
        Task<CloudBlockBlob> SaveBlockBlob(string containerName, byte[] blob, string blobTitle);
    }
}