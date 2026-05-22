using BuildingBlocks.Application.Commands;

namespace Modules.WorkItems.Application.Fields.UpdateFieldReadModelOnFieldUpdated;

public sealed record UpdateFieldReadModelOnFieldUpdatedCommand(
    Guid FieldId,
    string Name,
    bool IsOptional,
    Guid FieldTypeId,
    IReadOnlyList<Guid> WorkItemTypeIds) : ICommand;
