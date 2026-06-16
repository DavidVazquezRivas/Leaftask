using System.Globalization;
using BuildingBlocks.Application.Queries;
using Microsoft.EntityFrameworkCore;
using Modules.Notification.Application.ApprovalRequests.GetMy;
using Modules.Notification.Application.Notifications.GetMy;
using Modules.Notification.Domain.Entities.Approval;
using Modules.Notification.DrivenInfrastructure.Persistence;

namespace Modules.Notification.DrivenInfrastructure.Queries;

public sealed class GetMyApprovalsQueryService(NotificationsDbContext dbContext)
    : IGetMyApprovalsQueryService
{
    private static readonly IReadOnlyDictionary<string, CursorSortFieldDefinition<ApprovalRow>> SortFields =
        new Dictionary<string, CursorSortFieldDefinition<ApprovalRow>>(StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = new("id", row => row.Id, value => Guid.Parse(value), value => ((Guid)value).ToString("D")),
            ["createdAt"] = new("createdAt", row => row.CreatedAt,
                value => DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                value => ((DateTime)value).ToString("O", CultureInfo.InvariantCulture))
        };

    private static readonly IReadOnlyList<string> DefaultSort = ["createdAt:desc", "id:desc"];

    public async Task<PaginatedResult<ApprovalDto>> GetMyApprovalsAsync(
        Guid userId,
        int limit,
        string? cursor,
        CancellationToken cancellationToken = default)
    {
        // Find all PermissionName+ContextId combinations where this user has Full (level=2)
        var myFullPermissions = await dbContext.OrganizationPermissionReadModels
            .AsNoTracking()
            .Where(p => p.UserId == userId && p.Level == 2)
            .Select(p => new { p.OrganizationId, p.PermissionName })
            .ToListAsync(cancellationToken);

        if (myFullPermissions.Count == 0)
        {
            return new PaginatedResult<ApprovalDto>([], null, false);
        }

        // Load matching approval requests with their comments
        List<ApprovalRequest> approvals = await dbContext.ApprovalRequests
            .AsNoTracking()
            .Include(ar => ar.Requester)
            .Include(ar => ar.Comments)
                .ThenInclude(rc => rc.CreatedBy)
            .Where(ar => myFullPermissions.Any(p =>
                p.OrganizationId == ar.ContextId && p.PermissionName == ar.PermissionName))
            .ToListAsync(cancellationToken);

        List<ApprovalRow> rows = approvals
            .Select(ar => new ApprovalRow(
                ar.Id,
                ar.Status switch
                {
                    RequestStatus.Pending => "pending",
                    RequestStatus.Approved => "approved",
                    RequestStatus.Rejected => "rejected",
                    _ => ar.Status.ToString()
                },
                ar.ContextId,
                ar.TargetId,
                ar.Requester.Id,
                $"{ar.Requester.FirstName} {ar.Requester.LastName}",
                ar.CreatedAt,
                ar.Comments.Select(rc => new CommentRow(
                    rc.Id,
                    rc.Content,
                    rc.CreatedAt,
                    rc.CreatedBy.Id,
                    $"{rc.CreatedBy.FirstName} {rc.CreatedBy.LastName}")).ToArray()))
            .ToList();

        return CursorPaginationHelper.Paginate(
            rows,
            limit,
            cursor,
            [],
            SortFields,
            DefaultSort,
            row => new ApprovalDto(
                row.Id,
                row.Status,
                new SimpleReferenceDto(row.ContextId, "Organization"),
                new SimpleReferenceDto(row.TargetId, "Permission Request"),
                new SimpleReferenceDto(row.RequesterId, row.RequesterName),
                row.CreatedAt,
                row.Comments.Select(c => new ApprovalCommentDto(
                    c.Id,
                    c.Content,
                    c.CreatedAt,
                    new SimpleReferenceDto(c.CreatedById, c.CreatedByName))).ToArray()));

    }

    private sealed record ApprovalRow(
        Guid Id,
        string Status,
        Guid ContextId,
        Guid TargetId,
        Guid RequesterId,
        string RequesterName,
        DateTime CreatedAt,
        IReadOnlyCollection<CommentRow> Comments);

    private sealed record CommentRow(
        Guid Id,
        string Content,
        DateTime CreatedAt,
        Guid CreatedById,
        string CreatedByName);
}
