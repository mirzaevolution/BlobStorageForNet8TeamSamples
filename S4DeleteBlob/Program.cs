using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using System.Linq;

namespace S4DeleteBlob
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

        static async Task DeleteBlob(string blobName)
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

                //2. Get blob object and check whether it exists or not, if exists then delete it
                CloudBlob cloudBlob = blobContainer.GetBlockBlobReference(blobName);
                if (await cloudBlob.DeleteIfExistsAsync())
                {
                    Console.WriteLine("Blob has been deleted");
                }
                else
                {
                    Console.WriteLine("Blob is not found or doesn't exist");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        static void Main(string[] args)
        {

            string blobName = args.FirstOrDefault();
            DeleteBlob(blobName).GetAwaiter().GetResult();
            Console.ReadLine();
        }
    }
}
