using BuildingBlocks.Application.Commands;
using Modules.WorkItems.Domain.Entities.Field;
using Modules.WorkItems.Domain.Repositories;

namespace Modules.WorkItems.Application.Fields.DeleteFieldReadModelOnFieldDeleted;

public sealed class DeleteFieldReadModelOnFieldDeletedCommandHandler(IFieldRepository fieldRepository)
    : ICommandHandler<DeleteFieldReadModelOnFieldDeletedCommand>
{
    public async Task Handle(DeleteFieldReadModelOnFieldDeletedCommand request, CancellationToken cancellationToken)
    {
        FieldReadModel? fieldReadModel = await fieldRepository.GetFieldReadModelTrackedByIdAsync(request.FieldId, cancellationToken);
        if (fieldReadModel is null) return;

        fieldRepository.RemoveFieldReadModel(fieldReadModel);
        await fieldRepository.SaveChangesAsync(cancellationToken);
    }
}
