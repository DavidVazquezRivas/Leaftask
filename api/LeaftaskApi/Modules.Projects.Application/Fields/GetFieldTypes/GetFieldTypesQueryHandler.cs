using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.Projects.Application.Fields.GetFieldTypes;

public sealed class GetFieldTypesQueryHandler(IGetFieldTypesQueryService queryService)
    : IQueryHandler<GetFieldTypesQuery, Result<IReadOnlyList<FieldTypeDto>>>
{
    public async Task<Result<IReadOnlyList<FieldTypeDto>>> Handle(
        GetFieldTypesQuery query,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<FieldTypeDto> fieldTypes = await queryService.GetFieldTypesAsync(cancellationToken);
        return Result.Success(fieldTypes);
    }
}
