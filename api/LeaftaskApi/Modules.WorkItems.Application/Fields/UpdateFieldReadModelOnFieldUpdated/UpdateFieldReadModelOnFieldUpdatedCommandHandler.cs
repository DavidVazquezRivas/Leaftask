using BuildingBlocks.Application.Commands;
using Modules.WorkItems.Domain.Entities.Field;
using Modules.WorkItems.Domain.Entities.Properties;
using Modules.WorkItems.Domain.Repositories;

namespace Modules.WorkItems.Application.Fields.UpdateFieldReadModelOnFieldUpdated;

public sealed class UpdateFieldReadModelOnFieldUpdatedCommandHandler(
    IFieldRepository fieldRepository,
    IWorkItemConfigurationRepository configurationRepository)
    : ICommandHandler<UpdateFieldReadModelOnFieldUpdatedCommand>
{
    public async Task Handle(UpdateFieldReadModelOnFieldUpdatedCommand request, CancellationToken cancellationToken)
    {
        FieldTypeReadModel? fieldType = await fieldRepository.GetFieldTypeReadModelByIdAsync(request.FieldTypeId, cancellationToken);
        if (fieldType is null) return;

        List<WorkItemType> workItemTypes = request.WorkItemTypeIds.Count > 0
            ? await configurationRepository.GetTypesByIdsAsync(request.WorkItemTypeIds, cancellationToken)
            : [];

        FieldReadModel? fieldReadModel = await fieldRepository.GetFieldReadModelTrackedByIdAsync(request.FieldId, cancellationToken);

        if (fieldReadModel is null)
        {
            fieldReadModel = new(request.FieldId, request.Name, request.IsOptional, fieldType);
            fieldReadModel.SetAppliesTo(workItemTypes);
            await fieldRepository.AddFieldReadModelAsync(fieldReadModel, cancellationToken);
        }
        else
        {
            fieldReadModel.Update(request.Name, request.IsOptional, fieldType);
            fieldReadModel.SetAppliesTo(workItemTypes);
        }

        await fieldRepository.SaveChangesAsync(cancellationToken);
    }
}
