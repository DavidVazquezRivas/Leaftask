using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.Events;
using Modules.Organizations.Domain.Repositories;

namespace Modules.Organizations.Application.Members.UpdateRole;

public sealed class UpdateOrganizationMemberRoleCommandHandler(
    IOrganizationRepository organizationRepository,
    IOrganizationPermissionRepository organizationPermissionRepository)
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

        IReadOnlyCollection<OrganizationPermission> availablePermissions =
            await organizationPermissionRepository.GetAllAsync(cancellationToken);

        OrganizationRole? newRole = organization.Roles.FirstOrDefault(r => r.Id == request.RoleId);
        if (newRole is not null)
        {
            IReadOnlyCollection<OrganizationPermissionEntry> permissions = newRole.Permissions
                .Select(rp => new OrganizationPermissionEntry(
                    availablePermissions.First(p => p.Id == rp.OrganizationPermissionId).Name,
                    (int)rp.Level))
                .ToArray();
            organization.Raise(new OrganizationMemberRoleChangedDomainEvent(organization.Id, request.MemberId, permissions));
        }

        await organizationRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
