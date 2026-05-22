namespace BuildingBlocks.Application.Abstractions;

public record PresignedUploadResult(Uri PresignedUrl, Uri PublicUrl);

public interface IFileStorage
{
    Task<Uri> UploadAsync(string objectKey, Stream content, string contentType, long size, CancellationToken cancellationToken = default);
    Task DeleteAsync(string objectKey, CancellationToken cancellationToken = default);
    Task<PresignedUploadResult> GetPresignedPutUrlAsync(string objectKey, int expirySeconds = 900, CancellationToken cancellationToken = default);
}
