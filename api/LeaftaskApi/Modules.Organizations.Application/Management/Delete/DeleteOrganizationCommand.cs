using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;

namespace Modules.Organizations.Application.Management.Delete;

public sealed record DeleteOrganizationCommand(Guid Id) : ICommand<Result>;
