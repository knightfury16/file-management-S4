using Amazon.S3;
using Amazon.S3.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using s4.Models;

namespace s4.Controllers;

[ApiController]
[Route("[controller]")]
public class DownloadController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IAmazonS3 _s3Client;
    private readonly IOptions<AwsConfig> _awsConfig;

    public DownloadController(
        ILogger<WeatherForecastController> logger,
        IOptions<AwsConfig> awsConfig,
        IAmazonS3 s3Client
    )
    {
        _logger = logger;
        _awsConfig = awsConfig;
        _s3Client = s3Client;
    }

    [HttpGet("DirectDownload")]
    public async Task<IActionResult> DirectUpload(string key)
    {
        if (string.IsNullOrEmpty(key))
            return BadRequest("Give a valid key");

        var fileExists = await DoesS3ObjectExists(key);

        if (!fileExists)
            return NotFound(key);

        var res = await _s3Client.GetObjectAsync(_awsConfig.Value.BucketName, key);
        return File(res.ResponseStream, res.Headers.ContentType, res.Metadata["file-name"]);
    }

    private async Task<bool> DoesS3ObjectExists(string key)
    {
        try
        {
            var res = await _s3Client.GetObjectMetadataAsync(_awsConfig.Value.BucketName, key);
            return true;
        }
        catch (AmazonS3Exception ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                return false;
            else
                throw;
        }
    }
}
