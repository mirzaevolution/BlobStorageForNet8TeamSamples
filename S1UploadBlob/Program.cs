/* (C) 2020 - Mirza Ghulam Rasyid */
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using System.Linq;

namespace S1UploadBlob
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

        static async Task UploadBlob(string fileLocation)
        {
            try
            {
                if (!string.IsNullOrEmpty(fileLocation))
                {
                    if (File.Exists(fileLocation))
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

                        //2. Upload from specified file
                        //NB: You can also upload from Byte[] or Stream object


                        //3. Create blob name from file name
                        string blobName = Path.GetFileName(fileLocation);

                        CloudBlockBlob uploadedBlob =
                            blobContainer.GetBlockBlobReference(blobName);
                        Console.WriteLine($"Uploading {fileLocation}");
                        await uploadedBlob.UploadFromFileAsync(fileLocation);
                        Console.WriteLine($"Uploaded url: {uploadedBlob.Uri?.ToString()}");

                    }
                    else
                    {
                        Console.WriteLine("File does not exist");
                    }
                }
                else
                {
                    Console.WriteLine("File location cannot be empty!");
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
            UploadBlob(fileLocation).GetAwaiter().GetResult();
            Console.ReadLine();
        }
    }
}

