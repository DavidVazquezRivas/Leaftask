using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;

namespace Modules.Projects.Application.Management.Delete;

public sealed record DeleteProjectCommand(Guid ProjectId) : ICommand<Result>;
