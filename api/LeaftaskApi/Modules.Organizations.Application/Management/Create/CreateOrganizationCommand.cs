using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;

namespace Modules.Organizations.Application.Management.Create;

public record CreateOrganizationCommand(string Name, string Description, string Website)
    : ICommand<Result<BasicOrganizationResponse>>;
