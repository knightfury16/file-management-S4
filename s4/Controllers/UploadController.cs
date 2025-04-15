using Amazon.S3;
using Amazon.S3.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using s4.Models;

namespace s4.Controllers;

[ApiController]
[Route("[controller]")]
public class UploadController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IAmazonS3 _s3Client;
    private readonly IOptions<AwsConfig> _awsConfig;

    public UploadController(
        ILogger<WeatherForecastController> logger,
        IOptions<AwsConfig> awsConfig,
        IAmazonS3 s3Client
    )
    {
        _logger = logger;
        _awsConfig = awsConfig;
        _s3Client = s3Client;
    }

    [HttpPost("DirectUplaod")]
    public async Task<IActionResult> DirectUpload(IFormFile file)
    {
        if (file.Length == 0)
            return BadRequest("Select file to uplaod");

        var fileStream = file.OpenReadStream();
        var key = Guid.NewGuid();
        var objectRequest = new Amazon.S3.Model.PutObjectRequest
        {
            BucketName = _awsConfig.Value.BucketName,
            Key = $"Root/{key}",
            InputStream = fileStream,
            ContentType = file.ContentType,
            Metadata = { ["file-name"] = file.FileName },
        };
        var res = await _s3Client.PutObjectAsync(objectRequest);
        return Ok(key);
    }
}
