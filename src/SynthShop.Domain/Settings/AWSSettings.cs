namespace SynthShop.Domain.Settings;

public sealed class AWSSettings
{
    public required string BucketName { get; set; }
    public required string CloudFrontDomainUrl { get; set; }
}