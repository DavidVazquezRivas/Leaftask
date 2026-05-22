using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.Repositories;

namespace Modules.Organizations.Application.Members.UpdateRole;

public sealed class UpdateOrganizationMemberRoleCommandHandler(
    IOrganizationRepository organizationRepository)
    : ICommandHandler<UpdateOrganizationMemberRoleCommand, Result>
{
    public async Task<Result> Handle(UpdateOrganizationMemberRoleCommand request, CancellationToken cancellationToken)
    {
        Organization? organization = await organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return Result.Failure(OrganizationErrors.OrganizationNotFound);
        }

        Result updateResult = organization.UpdateMemberRole(request.MemberId, request.RoleId);
        if (updateResult.IsFailure)
        {
            return Result.Failure(updateResult.Error);
        }

        await organizationRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
