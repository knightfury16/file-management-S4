namespace s4.Models;

public class AwsConfig
{
    public string BucketName { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string AwsAccessKey { get; set; } = string.Empty;
    public string AwsSecretKey { get; set; } = string.Empty;
}
