using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.Projects.Application.Fields.GetProjectCustomFields;

public sealed record GetProjectCustomFieldsQuery(Guid ProjectId) : IQuery<Result<IReadOnlyList<CustomFieldDto>>>;
