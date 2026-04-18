using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.Repositories;

namespace Modules.Organizations.Application.Roles.GetRoles;

public sealed class GetOrganizationRolesQueryHandler(
    IOrganizationRepository organizationRepository,
    IGetOrganizationRolesQueryService service,
    IUserContext userContext)
    : IQueryHandler<GetOrganizationRolesQuery, Result<IReadOnlyList<OrganizationRoleDto>>>
{
    public async Task<Result<IReadOnlyList<OrganizationRoleDto>>> Handle(
        GetOrganizationRolesQuery request,
        CancellationToken cancellationToken)
    {
        Organization? organization = await organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return Result.Failure<IReadOnlyList<OrganizationRoleDto>>(OrganizationErrors.OrganizationNotFound);
        }

        bool isMember = organization.Invitations.Any(invitation =>
            invitation.UserId == userContext.UserId && invitation.Status == InvitationStatus.Accepted);

        if (!isMember)
        {
            return Result.Failure<IReadOnlyList<OrganizationRoleDto>>(OrganizationErrors.OrganizationMembershipRequired);
        }

        IReadOnlyList<OrganizationRoleDto> roles = await service.GetOrganizationRolesAsync(request.OrganizationId, cancellationToken);
        return Result.Success(roles);
    }
}
