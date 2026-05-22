using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.Repositories;

namespace Modules.Organizations.Application.Roles.GetMyPermissions;

public sealed class GetMyOrganizationPermissionsQueryHandler(
    IOrganizationRepository organizationRepository,
    IGetMyOrganizationPermissionsQueryService service,
    IUserContext userContext)
    : IQueryHandler<GetMyOrganizationPermissionsQuery, Result<IReadOnlyList<MyOrganizationPermissionDto>>>
{
    public async Task<Result<IReadOnlyList<MyOrganizationPermissionDto>>> Handle(
        GetMyOrganizationPermissionsQuery request,
        CancellationToken cancellationToken)
    {
        Organization? organization = await organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return Result.Failure<IReadOnlyList<MyOrganizationPermissionDto>>(OrganizationErrors.OrganizationNotFound);
        }

        bool isMember = organization.Invitations.Any(invitation =>
            invitation.UserId == userContext.UserId && invitation.Status == InvitationStatus.Accepted);

        if (!isMember)
        {
            return Result.Failure<IReadOnlyList<MyOrganizationPermissionDto>>(OrganizationErrors.OrganizationMembershipRequired);
        }

        IReadOnlyList<MyOrganizationPermissionDto> permissions =
            await service.GetMyPermissionsAsync(request.OrganizationId, userContext.UserId, cancellationToken);

        return Result.Success(permissions);
    }
}
