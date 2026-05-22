using BuildingBlocks.Application.Commands;

namespace Modules.WorkItems.Application.Fields.DeleteFieldReadModelOnFieldDeleted;

public sealed record DeleteFieldReadModelOnFieldDeletedCommand(Guid FieldId) : ICommand;
