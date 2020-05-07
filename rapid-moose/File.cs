using System;
using System.Threading.Tasks;

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.IO;

namespace rapid_moose
{
    public class File
    {
        public static async void UploadUserFile(int userID, string data)
        {
            string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("users");
            BlobClient blobClient = containerClient.GetBlobClient(userID + ".json");

            using (Stream stringStream = ToStream(data))
            {
                await blobClient.UploadAsync(stringStream, true);
                stringStream.Close();
            }
        }

        public static async Task<string> DownloadUserFile(int userID)
        {
            string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("users");
            BlobClient blobClient = containerClient.GetBlobClient(userID + ".json");

            BlobDownloadInfo download = await blobClient.DownloadAsync();

            using Stream stringStream = new MemoryStream();
            
            string data = "";

            using (var reader = new StreamReader(download.Content))
            {
                data += reader.ReadToEnd();
            }

            stringStream.Close();


            return (data);
        }

        public static Stream ToStream(string str)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
