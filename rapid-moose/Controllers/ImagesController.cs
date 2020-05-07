using System;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace rapid_moose.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<Blob>> Get([FromHeader] int ApiAccessKey)
        {
            if (!Session.CheckSession(ApiAccessKey))
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            int userId = Session.LookUpUserId(ApiAccessKey);

            if (userId == -1)
            {
                return BadRequest();
            }

            string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("images");
            string path = rapid_moose.User.GetIconName(userId);
            BlobClient blobClient = containerClient.GetBlobClient(path);

            BlobDownloadInfo download = await blobClient.DownloadAsync();
            
            return File(download.Content, "image/" + path.Substring(path.LastIndexOf(".") + 1));
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromHeader] int ApiAccessKey)
        {
            if (!Session.CheckSession(ApiAccessKey))
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            int userId = Session.LookUpUserId(ApiAccessKey);

            if (userId == -1)
            {
                return BadRequest();
            }

            string path = userId + "." + Request.ContentType.Substring(Request.ContentType.LastIndexOf("/") + 1);
            rapid_moose.User.SetIconName(userId, path);

            try
            {
                string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("images");

                string pathToDelete = rapid_moose.User.GetIconName(userId);
                BlobClient blobClientToDelete = containerClient.GetBlobClient(pathToDelete);
                await blobClientToDelete.DeleteIfExistsAsync();

                BlobClient blobClient = containerClient.GetBlobClient(path);

                await blobClient.UploadAsync(Request.Body);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

            return Ok();
        }
    }
}
