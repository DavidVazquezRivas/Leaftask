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
        // Collect (contextId, permissionName) pairs where the user has Full (level=2)
        var orgPerms = await dbContext.OrganizationPermissionReadModels
            .AsNoTracking()
            .Where(p => p.UserId == userId && p.Level == 2)
            .Select(p => new { ContextId = p.OrganizationId, p.PermissionName })
            .ToListAsync(cancellationToken);

        var projectPerms = await dbContext.ProjectPermissionReadModels
            .AsNoTracking()
            .Where(p => p.UserId == userId && p.Level == 2)
            .Select(p => new { ContextId = p.ProjectId, p.PermissionName })
            .ToListAsync(cancellationToken);

        var allPerms = orgPerms.Concat(projectPerms).ToList();

        if (allPerms.Count == 0)
            return new PaginatedResult<ApprovalDto>([], null, false);

        List<Guid> contextIds = allPerms.Select(p => p.ContextId).Distinct().ToList();
        List<string> permNames = allPerms.Select(p => p.PermissionName).Distinct().ToList();

        List<ApprovalRequest> approvals = (await dbContext.ApprovalRequests
            .AsNoTracking()
            .Include(ar => ar.Requester)
            .Include(ar => ar.Comments)
                .ThenInclude(rc => rc.CreatedBy)
            .Where(ar => contextIds.Contains(ar.ContextId) && permNames.Contains(ar.PermissionName))
            .ToListAsync(cancellationToken))
            .Where(ar => allPerms.Any(p => p.ContextId == ar.ContextId && p.PermissionName == ar.PermissionName))
            .ToList();

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
                ar.ContextType switch
                {
                    ContextType.Organization => "organization",
                    ContextType.Project => "project",
                    _ => "unknown"
                },
                ar.PermissionName,
                ar.ActionType,
                ar.ActionPayload,
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
                row.ContextType,
                row.PermissionName,
                row.ActionType,
                row.ActionPayload,
                new SimpleReferenceDto(row.ContextId, row.ContextId.ToString()),
                new SimpleReferenceDto(row.TargetId, row.TargetId.ToString()),
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
        string ContextType,
        string PermissionName,
        string? ActionType,
        string? ActionPayload,
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
