using Amazon.S3;
using Amazon.S3.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using s4.Models;

namespace s4.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing",
        "Bracing",
        "Chilly",
        "Cool",
        "Mild",
        "Warm",
        "Balmy",
        "Hot",
        "Sweltering",
        "Scorching",
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IAmazonS3 _s3Client;
    private readonly IOptions<AwsConfig> _awsConfig;

    public WeatherForecastController(
        ILogger<WeatherForecastController> logger,
        IOptions<AwsConfig> awsConfig,
        IAmazonS3 s3Client
    )
    {
        _logger = logger;
        _awsConfig = awsConfig;
        _s3Client = s3Client;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable
            .Range(1, 5)
            .Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)],
            })
            .ToArray();
    }

    [HttpGet("GetConfig")]
    public IActionResult GetConfig()
    {
        var response = new
        {
            bucketName = _awsConfig.Value.BucketName,
            region = _awsConfig.Value.Region,
            awsAccessKey = _awsConfig.Value.AwsAccessKey,
            awsSecretKey = _awsConfig.Value.AwsSecretKey,
        };

        return Ok(response);
    }

    [HttpGet("DoesBucketExists")]
    public async Task<IActionResult> GetIfBucketExists()
    {
        var response = await AmazonS3Util.DoesS3BucketExistV2Async(
            _s3Client,
            _awsConfig.Value.BucketName
        );
        return Ok(response);
    }
}
