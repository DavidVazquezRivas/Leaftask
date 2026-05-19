using BuildingBlocks.Application.Commands;

namespace Modules.WorkItems.Application.Fields.CreateFieldReadModelOnFieldCreated;

public sealed record CreateFieldReadModelOnFieldCreatedCommand(
    Guid FieldId,
    string Name,
    bool IsOptional,
    Guid FieldTypeId,
    IReadOnlyList<Guid> WorkItemTypeIds) : ICommand;
