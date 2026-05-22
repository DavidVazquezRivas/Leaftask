using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using BuildingBlocks.Application.Abstractions;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace BuildingBlocks.DrivenInfrastructure.Storage;

public sealed class MinioFileStorage(IMinioClient minioClient, IOptions<MinioOptions> options) : IFileStorage
{
    private readonly MinioOptions _options = options.Value;
    private volatile bool _bucketInitialized;

    // Separate client used only for presigned URL generation, configured with the public endpoint
    // so that the Host in the signed URL matches what the browser actually hits.
    private readonly Lazy<IMinioClient> _presignedClient = new(() =>
    {
        string publicEndpoint = string.IsNullOrEmpty(options.Value.PublicEndpoint)
            ? options.Value.Endpoint
            : options.Value.PublicEndpoint;

        Uri publicUri = new(publicEndpoint.StartsWith("http", StringComparison.OrdinalIgnoreCase)
            ? publicEndpoint
            : $"http://{publicEndpoint}");

        IMinioClient builder = new MinioClient()
            .WithEndpoint(publicUri.Host, publicUri.Port)
            .WithCredentials(options.Value.AccessKey, options.Value.SecretKey);

        if (string.Equals(publicUri.Scheme, "https", StringComparison.OrdinalIgnoreCase))
            builder = ((MinioClient)builder).WithSSL();

        return builder.Build();
    });

    public async Task<Uri> UploadAsync(
        string objectKey,
        Stream content,
        string contentType,
        long size,
        CancellationToken cancellationToken = default)
    {
        await EnsureBucketAsync(cancellationToken);

        await minioClient.PutObjectAsync(
            new PutObjectArgs()
                .WithBucket(_options.BucketName)
                .WithObject(objectKey)
                .WithStreamData(content)
                .WithObjectSize(size)
                .WithContentType(contentType),
            cancellationToken);

        return BuildPublicUrl(objectKey);
    }

    public async Task DeleteAsync(string objectKey, CancellationToken cancellationToken = default)
    {
        await minioClient.RemoveObjectAsync(
            new RemoveObjectArgs()
                .WithBucket(_options.BucketName)
                .WithObject(objectKey),
            cancellationToken);
    }

    public async Task<PresignedUploadResult> GetPresignedPutUrlAsync(
        string objectKey,
        int expirySeconds = 900,
        CancellationToken cancellationToken = default)
    {
        await EnsureBucketAsync(cancellationToken);

        // Use the public-endpoint client so the Host in the signature matches what the browser hits.
        string rawPresignedUrl = await _presignedClient.Value.PresignedPutObjectAsync(
            new PresignedPutObjectArgs()
                .WithBucket(_options.BucketName)
                .WithObject(objectKey)
                .WithExpiry(expirySeconds));

        return new PresignedUploadResult(new Uri(rawPresignedUrl), BuildPublicUrl(objectKey));
    }

    private Uri BuildPublicUrl(string objectKey)
    {
        string publicEndpointStr = string.IsNullOrEmpty(_options.PublicEndpoint)
            ? $"http://{_options.Endpoint}"
            : _options.PublicEndpoint;

        return new Uri($"{publicEndpointStr.TrimEnd('/')}/{_options.BucketName}/{objectKey}");
    }

    private async Task EnsureBucketAsync(CancellationToken cancellationToken)
    {
        if (_bucketInitialized) return;

        bool exists = await minioClient.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(_options.BucketName),
            cancellationToken);

        if (!exists)
        {
            await minioClient.MakeBucketAsync(
                new MakeBucketArgs().WithBucket(_options.BucketName),
                cancellationToken);

            string policy = $$"""
                {
                  "Version": "2012-10-17",
                  "Statement": [
                    {
                      "Effect": "Allow",
                      "Principal": {"AWS": ["*"]},
                      "Action": ["s3:GetObject"],
                      "Resource": ["arn:aws:s3:::{{_options.BucketName}}/*"]
                    }
                  ]
                }
                """;

            await minioClient.SetPolicyAsync(
                new SetPolicyArgs()
                    .WithBucket(_options.BucketName)
                    .WithPolicy(policy),
                cancellationToken);
        }

        // Configure CORS so browsers can PUT directly via presigned URLs.
        // MinIO SDK 6.x doesn't expose SetBucketCorsAsync, so we use the S3 REST API directly.
        await SetBucketCorsAsync(cancellationToken);

        _bucketInitialized = true;
    }

    private async Task SetBucketCorsAsync(CancellationToken cancellationToken)
    {
        const string corsXml = """
            <CORSConfiguration xmlns="http://s3.amazonaws.com/doc/2006-03-01/">
              <CORSRule>
                <AllowedOrigin>*</AllowedOrigin>
                <AllowedMethod>GET</AllowedMethod>
                <AllowedMethod>PUT</AllowedMethod>
                <AllowedMethod>HEAD</AllowedMethod>
                <AllowedMethod>DELETE</AllowedMethod>
                <AllowedHeader>*</AllowedHeader>
                <ExposeHeader>ETag</ExposeHeader>
                <MaxAgeSeconds>3600</MaxAgeSeconds>
              </CORSRule>
            </CORSConfiguration>
            """;

        byte[] bodyBytes = Encoding.UTF8.GetBytes(corsXml);
#pragma warning disable CA1308
        string payloadHash = Convert.ToHexString(SHA256.HashData(bodyBytes)).ToLowerInvariant();
#pragma warning restore CA1308

        DateTime now = DateTime.UtcNow;
        string amzDate = now.ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture);
        string dateStamp = now.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        const string region = "us-east-1";
        const string service = "s3";

        string scheme = _options.UseSSL ? "https" : "http";
        string rawEndpoint = _options.Endpoint.StartsWith("http", StringComparison.OrdinalIgnoreCase)
            ? _options.Endpoint
            : $"{scheme}://{_options.Endpoint}";
        string host = new Uri(rawEndpoint).Authority;

        // Canonical headers must be sorted alphabetically by name
        string canonicalHeaders = $"host:{host}\nx-amz-content-sha256:{payloadHash}\nx-amz-date:{amzDate}\n";
        const string signedHeaders = "host;x-amz-content-sha256;x-amz-date";

        string canonicalRequest =
            $"PUT\n/{_options.BucketName}\ncors=\n{canonicalHeaders}\n{signedHeaders}\n{payloadHash}";

        string credentialScope = $"{dateStamp}/{region}/{service}/aws4_request";
#pragma warning disable CA1308
        string hashedCanonical = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonicalRequest))).ToLowerInvariant();
        string stringToSign = $"AWS4-HMAC-SHA256\n{amzDate}\n{credentialScope}\n{hashedCanonical}";

        byte[] kDate = HMACSHA256.HashData(Encoding.UTF8.GetBytes($"AWS4{_options.SecretKey}"), Encoding.UTF8.GetBytes(dateStamp));
        byte[] kRegion = HMACSHA256.HashData(kDate, Encoding.UTF8.GetBytes(region));
        byte[] kService = HMACSHA256.HashData(kRegion, Encoding.UTF8.GetBytes(service));
        byte[] kSigning = HMACSHA256.HashData(kService, Encoding.UTF8.GetBytes("aws4_request"));
        string signature = Convert.ToHexString(HMACSHA256.HashData(kSigning, Encoding.UTF8.GetBytes(stringToSign))).ToLowerInvariant();
#pragma warning restore CA1308

        string authorization = $"AWS4-HMAC-SHA256 Credential={_options.AccessKey}/{credentialScope}, SignedHeaders={signedHeaders}, Signature={signature}";

        using HttpClient httpClient = new();
        using HttpRequestMessage request = new(HttpMethod.Put, $"{rawEndpoint}/{_options.BucketName}?cors")
        {
            Content = new ByteArrayContent(bodyBytes),
        };
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
        request.Headers.TryAddWithoutValidation("x-amz-date", amzDate);
        request.Headers.TryAddWithoutValidation("x-amz-content-sha256", payloadHash);
        request.Headers.TryAddWithoutValidation("Authorization", authorization);

        // Non-fatal — bucket may already have CORS configured, or MinIO may have CORS enabled globally
        try
        {
            await httpClient.SendAsync(request, cancellationToken);
        }
        catch (Exception)
        {
            // ignored
        }
    }
}
