using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.Projects.Application.Members.GetCount;

public sealed record GetProjectMemberCountQuery(Guid ProjectId) : IQuery<Result<ProjectMemberCountDto>>;
