namespace Modules.WorkItems.DrivingInfrastructure.Models.Requests;

public sealed record AddCommentRequest(string Content, IReadOnlyList<Guid>? AttachmentIds);
