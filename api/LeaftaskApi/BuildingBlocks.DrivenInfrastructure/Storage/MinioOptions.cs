namespace BuildingBlocks.DrivenInfrastructure.Storage;

public sealed class MinioOptions
{
    public string Endpoint { get; init; } = string.Empty;
    public string AccessKey { get; init; } = string.Empty;
    public string SecretKey { get; init; } = string.Empty;
    public string BucketName { get; init; } = string.Empty;
    public bool UseSSL { get; init; }
    public string PublicEndpoint { get; init; } = string.Empty;
}
