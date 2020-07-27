using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using System.Linq;
namespace S2ListAllBlobs
{
    class Program
    {
        private static string _containerRootName = "documents";
        private static string _connectionString = "<BLOB_CONNECTION_STRING>";
        private static CloudBlobClient _blobClient;

        static void Init()
        {
            try
            {
                //1. Create cloud storage account object
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_connectionString);

                //2. Initialize cloud blob client
                _blobClient = cloudStorageAccount.CreateCloudBlobClient();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        static Program()
        {
            Init();
        }

        static async Task ListBlobs()
        {
            try
            {
                //1. Get/Create container reference
                CloudBlobContainer blobContainer =
                    _blobClient.GetContainerReference(_containerRootName);

                await blobContainer.CreateIfNotExistsAsync(
                    accessType: BlobContainerPublicAccessType.Blob,
                    options: new BlobRequestOptions
                    {
                        DisableContentMD5Validation = true
                    },
                    operationContext: null);

                //2. List all blobs using flat query
                IEnumerable<IListBlobItem> allItems = blobContainer.ListBlobs(prefix: "", useFlatBlobListing: true);
                foreach (var item in allItems)
                {
                    //Check whether or not current item is directory
                    //if (item is CloudBlobDirectory)
                    //{
                    //    CloudBlobDirectory current = item as CloudBlobDirectory;
                    //    Console.WriteLine($"{current.Prefix}");
                    //}
                    //Check whether or not current item is blob item
                    if (item is CloudBlob)
                    {
                        CloudBlob current = item as CloudBlob;
                        Console.WriteLine($"{current.Name}");
                    }
                }

                Console.WriteLine();

                //3. Get blob by name
                allItems = blobContainer.ListBlobs(prefix: "", useFlatBlobListing: true)
                    .OfType<CloudBlob>()
                    .Where(c => c.Name.Contains("york", StringComparison.InvariantCultureIgnoreCase))
                    .ToList();

                foreach (var item in allItems)
                {
                    if (item is CloudBlob)
                    {
                        CloudBlob current = item as CloudBlob;
                        Console.WriteLine($"{current.Name}");
                    }
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        static void Main(string[] args)
        {
            ListBlobs().GetAwaiter().GetResult();
            Console.WriteLine("Press ENTER to exit");
            Console.ReadLine();
        }
    }
}
