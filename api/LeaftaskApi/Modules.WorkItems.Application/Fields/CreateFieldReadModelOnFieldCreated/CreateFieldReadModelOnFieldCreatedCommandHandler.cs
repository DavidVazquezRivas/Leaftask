using BuildingBlocks.Application.Commands;
using Modules.WorkItems.Domain.Entities.Field;
using Modules.WorkItems.Domain.Entities.Properties;
using Modules.WorkItems.Domain.Repositories;

namespace Modules.WorkItems.Application.Fields.CreateFieldReadModelOnFieldCreated;

public sealed class CreateFieldReadModelOnFieldCreatedCommandHandler(
    IFieldRepository fieldRepository,
    IWorkItemConfigurationRepository configurationRepository)
    : ICommandHandler<CreateFieldReadModelOnFieldCreatedCommand>
{
    public async Task Handle(CreateFieldReadModelOnFieldCreatedCommand request, CancellationToken cancellationToken)
    {
        bool exists = await fieldRepository.GetFieldReadModelByIdAsync(request.FieldId, cancellationToken) is not null;
        if (exists) return;

        FieldTypeReadModel? fieldType = await fieldRepository.GetFieldTypeReadModelByIdAsync(request.FieldTypeId, cancellationToken);
        if (fieldType is null) return;

        List<WorkItemType> workItemTypes = request.WorkItemTypeIds.Count > 0
            ? await configurationRepository.GetTypesByIdsAsync(request.WorkItemTypeIds, cancellationToken)
            : [];

        FieldReadModel fieldReadModel = new(request.FieldId, request.Name, request.IsOptional, fieldType);
        fieldReadModel.SetAppliesTo(workItemTypes);

        await fieldRepository.AddFieldReadModelAsync(fieldReadModel, cancellationToken);
        await fieldRepository.SaveChangesAsync(cancellationToken);
    }
}
