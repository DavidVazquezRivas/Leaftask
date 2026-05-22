using System.Globalization;
using BuildingBlocks.Application.Queries;
using Microsoft.EntityFrameworkCore;
using Modules.WorkItems.Application.Comments;
using Modules.WorkItems.Application.Comments.List;
using Modules.WorkItems.DrivenInfrastructure.Persistence;

namespace Modules.WorkItems.DrivenInfrastructure.Queries;

public sealed class GetCommentsQueryService(WorkItemsDbContext dbContext) : IGetCommentsQueryService
{
    private const string IdSort = "id:asc";

    private static readonly IReadOnlyDictionary<string, CursorSortFieldDefinition<CommentRow>> SortFields =
        new Dictionary<string, CursorSortFieldDefinition<CommentRow>>(StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = new("id", row => row.Id, value => Guid.Parse(value), value => ((Guid)value).ToString("D")),
            ["createdAt"] = new("createdAt", row => row.CreatedAt,
                value => DateTime.Parse(value, CultureInfo.InvariantCulture),
                value => ((DateTime)value).ToString("O", CultureInfo.InvariantCulture))
        };

    private static readonly IReadOnlyList<string> DefaultSort = ["createdAt:asc", "id:asc"];

    public async Task<PaginatedResult<CommentDto>> GetCommentsAsync(
        Guid projectId,
        Guid workItemId,
        int limit,
        string? cursor,
        IReadOnlyCollection<string> sort,
        CancellationToken cancellationToken = default)
    {
        List<CommentRow> rows = await dbContext.WorkItemComments
            .AsNoTracking()
            .Where(c => EF.Property<Guid>(c, "work_item_id") == workItemId)
            .Select(c => new CommentRow(
                c.Id,
                c.Content,
                c.CreatedAt,
                c.User.Id,
                c.User.FirstName,
                c.User.LastName))
            .ToListAsync(cancellationToken);

        List<AttachmentProjection> attachments = await dbContext.Attachments
            .AsNoTracking()
            .Where(a => EF.Property<Guid>(a, "work_item_id") == workItemId
                     && EF.Property<Guid?>(a, "comment_id") != null)
            .Select(a => new AttachmentProjection(
                a.Id,
                a.FileName,
                a.FileUrl,
                EF.Property<Guid?>(a, "comment_id")))
            .ToListAsync(cancellationToken);

        Dictionary<Guid, List<AttachmentProjection>> attachmentsByComment = attachments
            .GroupBy(a => a.CommentId!.Value)
            .ToDictionary(g => g.Key, g => g.ToList());

        IReadOnlyCollection<string> effectiveSort = NormalizeSort(sort);

        return CursorPaginationHelper.Paginate(
            rows,
            limit,
            cursor,
            effectiveSort,
            SortFields,
            DefaultSort,
            row => new CommentDto(
                row.Id,
                new CommentAuthorDto(row.AuthorId, $"{row.AuthorFirstName} {row.AuthorLastName}".Trim()),
                row.Content,
                row.CreatedAt,
                attachmentsByComment.TryGetValue(row.Id, out List<AttachmentProjection>? atts)
                    ? atts.Select(a => new CommentAttachmentDto(a.Id, a.FileName, a.Url)).ToList()
                    : []));
    }

    private static IReadOnlyCollection<string> NormalizeSort(IReadOnlyCollection<string> sort)
    {
        IReadOnlyCollection<string> baseSort = sort.Count == 0 ? DefaultSort : sort;
        return baseSort.Any(s => s.StartsWith("id:", StringComparison.OrdinalIgnoreCase))
            ? baseSort
            : [.. baseSort, IdSort];
    }

    private sealed record CommentRow(
        Guid Id,
        string Content,
        DateTime CreatedAt,
        Guid AuthorId,
        string AuthorFirstName,
        string AuthorLastName);

    private sealed record AttachmentProjection(
        Guid Id,
        string FileName,
        Uri Url,
        Guid? CommentId);
}
