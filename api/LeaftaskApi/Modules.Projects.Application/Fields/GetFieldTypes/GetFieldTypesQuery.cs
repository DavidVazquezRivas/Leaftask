using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.Projects.Application.Fields.GetFieldTypes;

public sealed record GetFieldTypesQuery : IQuery<Result<IReadOnlyList<FieldTypeDto>>>;
