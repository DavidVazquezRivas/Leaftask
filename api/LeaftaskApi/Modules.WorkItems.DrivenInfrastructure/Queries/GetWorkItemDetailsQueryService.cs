using Microsoft.EntityFrameworkCore;
using Modules.WorkItems.Application.WorkItems;
using Modules.WorkItems.Application.WorkItems.GetWorkItemDetails;
using Modules.WorkItems.DrivenInfrastructure.Persistence;

namespace Modules.WorkItems.DrivenInfrastructure.Queries;

public sealed class GetWorkItemDetailsQueryService(WorkItemsDbContext dbContext)
    : IGetWorkItemDetailsQueryService
{
    public async Task<WorkItemDetailDto?> GetWorkItemDetailsAsync(
        Guid projectId,
        Guid workItemId,
        CancellationToken cancellationToken = default)
    {
        WorkItemRow? item = await dbContext.WorkItems
            .AsNoTracking()
            .Where(wi => wi.Id == workItemId
                         && EF.Property<Guid>(wi, "project_read_model_id") == projectId)
            .Select(wi => new WorkItemRow(
                wi.Id,
                wi.Code,
                wi.Project.Abbreviation,
                wi.Title,
                wi.Description,
                wi.LimitDate,
                wi.Estimation,
                wi.Progress,
                wi.Status.Id,
                wi.Type.Id,
                wi.Asignee != null ? wi.Asignee.Id : (Guid?)null,
                wi.Asignee != null ? wi.Asignee.FirstName : null,
                wi.Asignee != null ? wi.Asignee.LastName : null,
                wi.ParentId))
            .FirstOrDefaultAsync(cancellationToken);

        if (item is null)
        {
            return null;
        }

        List<CommentRow> comments = await dbContext.WorkItemComments
            .AsNoTracking()
            .Where(c => EF.Property<Guid>(c, "work_item_id") == workItemId)
            .OrderBy(c => c.CreatedAt)
            .Select(c => new CommentRow(
                c.Id,
                c.User.Id,
                c.User.FirstName,
                c.User.LastName,
                c.Content,
                c.CreatedAt))
            .ToListAsync(cancellationToken);

        List<AttachmentRow> attachments = await dbContext.Attachments
            .AsNoTracking()
            .Where(a => EF.Property<Guid>(a, "work_item_id") == workItemId)
            .Select(a => new AttachmentRow(
                a.Id,
                a.FileName,
                a.FileUrl.ToString(),
                EF.Property<Guid?>(a, "comment_id")))
            .ToListAsync(cancellationToken);

        List<LogRow> logs = await dbContext.ActivityLogs
            .AsNoTracking()
            .Where(l => EF.Property<Guid>(l, "work_item_id") == workItemId)
            .OrderByDescending(l => l.CreatedAt)
            .Select(l => new LogRow(
                l.CreatedAt,
                l.UserReadModel.Id,
                l.UserReadModel.FirstName,
                l.UserReadModel.LastName,
                l.FieldName,
                l.OldValue,
                l.NewValue))
            .ToListAsync(cancellationToken);

        List<CustomFieldRow> customFields = await dbContext.FieldValues
            .AsNoTracking()
            .Where(fv => EF.Property<Guid>(fv, "work_item_id") == workItemId)
            .Select(fv => new CustomFieldRow(
                fv.Field.Id,
                fv.Field.Name,
                fv.Value,
                EF.Property<Guid>(fv.Field, "field_type_read_model_id")))
            .ToListAsync(cancellationToken);

        DedicationRow dedication = await dbContext.WorkLogs
            .AsNoTracking()
            .Where(wl => EF.Property<Guid>(wl, "work_item_id") == workItemId)
            .GroupBy(_ => 1)
            .Select(g => new DedicationRow((float)g.Sum(wl => wl.Hours), g.Count()))
            .FirstOrDefaultAsync(cancellationToken) ?? new DedicationRow(0f, 0);

        Dictionary<Guid, List<AttachmentRow>> commentAttachments = attachments
            .Where(a => a.CommentId.HasValue)
            .GroupBy(a => a.CommentId!.Value)
            .ToDictionary(g => g.Key, g => g.ToList());

        List<WorkItemAttachmentDto> rootAttachments = attachments
            .Where(a => !a.CommentId.HasValue)
            .Select(a => new WorkItemAttachmentDto(a.Id, a.FileName, new Uri(a.Url)))
            .ToList();

        return new WorkItemDetailDto(
            item.Id,
            $"{item.ProjectAbbreviation}-{item.Code}",
            item.Title,
            item.Description,
            item.LimitDate,
            item.AssigneeId.HasValue
                ? new WorkItemAssigneeDetailDto(
                    item.AssigneeId.Value,
                    $"{item.AssigneeFirstName} {item.AssigneeLastName}")
                : null,
            item.Estimation,
            new WorkItemDedicationDto(dedication.Total, dedication.Registers),
            item.Progress / 100f,
            item.TypeId,
            item.StatusId,
            item.ParentId,
            rootAttachments,
            comments.Select(c => new WorkItemCommentDetailDto(
                c.Id,
                new WorkItemAssigneeDetailDto(c.AuthorId, $"{c.AuthorFirstName} {c.AuthorLastName}"),
                c.Content,
                c.CreatedAt,
                commentAttachments.TryGetValue(c.Id, out List<AttachmentRow>? ca)
                    ? ca.Select(a => new WorkItemAttachmentDto(a.Id, a.FileName, new Uri(a.Url))).ToList()
                    : [])).ToList(),
            logs.Select(l => new WorkItemLogEntryDto(
                l.Timestamp,
                new WorkItemAssigneeDetailDto(l.UserId, $"{l.UserFirstName} {l.UserLastName}"),
                l.FieldName,
                new WorkItemLogValueDto(l.OldValue),
                new WorkItemLogValueDto(l.NewValue))).ToList(),
            customFields.Select(cf => new WorkItemCustomFieldDto(cf.FieldId, cf.Name, cf.Value, cf.TypeId)).ToList());
    }

    private sealed record WorkItemRow(
        Guid Id,
        int Code,
        string ProjectAbbreviation,
        string Title,
        string Description,
        DateTime LimitDate,
        decimal Estimation,
        int Progress,
        Guid StatusId,
        Guid TypeId,
        Guid? AssigneeId,
        string? AssigneeFirstName,
        string? AssigneeLastName,
        Guid? ParentId);

    private sealed record CommentRow(
        Guid Id,
        Guid AuthorId,
        string AuthorFirstName,
        string AuthorLastName,
        string Content,
        DateTime CreatedAt);

    private sealed record AttachmentRow(Guid Id, string FileName, string Url, Guid? CommentId);

    private sealed record LogRow(
        DateTime Timestamp,
        Guid UserId,
        string UserFirstName,
        string UserLastName,
        string FieldName,
        string OldValue,
        string NewValue);

    private sealed record DedicationRow(float Total, int Registers);

    private sealed record CustomFieldRow(Guid FieldId, string Name, string Value, Guid TypeId);
}
