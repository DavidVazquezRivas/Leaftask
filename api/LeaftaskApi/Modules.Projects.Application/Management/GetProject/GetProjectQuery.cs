using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using Modules.Projects.Application.Management.Create;

namespace Modules.Projects.Application.Management.GetProject;

public sealed record GetProjectQuery(Guid ProjectId) : IQuery<Result<ProjectResponse>>;
