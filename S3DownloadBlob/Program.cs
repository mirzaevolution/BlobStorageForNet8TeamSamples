using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using System.Linq;

namespace S3DownloadBlob
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

        static async Task DownloadBlob(string blobName)
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

                //2. Get blob object and check whether it exists or not
                CloudBlob cloudBlob = blobContainer.GetBlobReference(blobName);
                if (await cloudBlob.ExistsAsync())
                {
                    Console.WriteLine("Downloading...");
                    string path = Path.Combine(Directory.GetCurrentDirectory(), blobName);
                    await cloudBlob.DownloadToFileAsync(path, FileMode.Create);
                    Console.WriteLine($"Downloaded to: {path}");
                }
                else
                {
                    Console.WriteLine("Blob doesn't exist!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        static void Main(string[] args)
        {

            string fileLocation = args.FirstOrDefault();
            DownloadBlob(fileLocation).GetAwaiter().GetResult();
            Console.ReadLine();
        }
    }
}
