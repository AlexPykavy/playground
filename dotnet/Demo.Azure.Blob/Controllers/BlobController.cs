using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web.Resource;

namespace Demo.Azure.Blob.Controllers;

[ApiController]
[Route("[controller]")]
public class BlobController : ControllerBase
{
    private readonly BlobContainerClient _blobContainerClient;
    private readonly ILogger<BlobController> _logger;

    public BlobController(BlobServiceClient blobServiceClient, IOptions<StorageSettings> settings, ILogger<BlobController> logger)
    {
        _blobContainerClient = blobServiceClient.GetBlobContainerClient(settings.Value.ContainerName);
        _logger = logger;

        _blobContainerClient.CreateIfNotExists();
    }

    [HttpGet("file/{filename}")]
    public async Task<IActionResult> Get(string filename, bool asStream)
    {
        if (filename == null)
        {
            return BadRequest("No file specified");
        }

        if (asStream)
        {
            var options = new BlobOpenReadOptions(false)
            {
                BufferSize = 2 * 1024 * 1024
            };

            return File(await _blobContainerClient.GetBlobClient(filename).OpenReadAsync(options), "application/octet-stream", filename);
        }

        var ms = new MemoryStream();
        await _blobContainerClient.GetBlobClient(filename).DownloadToAsync(ms);

        ms.Flush();
        ms.Position = 0;

        return File(ms, "application/octet-stream", filename);
    }

    [HttpPost("file")]
    [RequestSizeLimit(int.MaxValue)]
    [RequestFormLimits(MultipartBodyLengthLimit = 500 * 1024 * 1024)]
    public async Task<IActionResult> PostAsStream(IFormFile file, bool asStream)
    {
        if (file == null)
        {
            return BadRequest("No file specified");
        }

        if (asStream)
        {
            var options = new BlobUploadOptions
            {
                TransferOptions = new StorageTransferOptions
                {
                    InitialTransferSize = 5 * 1024 * 1024,
                    MaximumConcurrency = 2,
                    MaximumTransferSize = 2 * 1024 * 1024
                }
            };

            return Ok(await _blobContainerClient.GetBlobClient(file.FileName).UploadAsync(file.OpenReadStream(), options));
        }

        var ms = new MemoryStream();

        file.OpenReadStream().CopyTo(ms);

        ms.Flush();
        ms.Position = 0;

        return Ok(await _blobContainerClient.GetBlobClient(file.FileName).UploadAsync(ms));
    }
}
