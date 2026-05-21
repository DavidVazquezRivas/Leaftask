namespace Modules.WorkItems.DrivingInfrastructure.Models.Requests;

public sealed record UpdateCommentRequest(string? Content, IReadOnlyList<Guid>? AttachmentIds);
