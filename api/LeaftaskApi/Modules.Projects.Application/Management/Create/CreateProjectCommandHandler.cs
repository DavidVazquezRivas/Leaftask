using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Entities.Member;
using Modules.Projects.Domain.Entities.Owner;
using Modules.Projects.Domain.Entities.Role;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;

namespace Modules.Projects.Application.Management.Create;

public sealed class CreateProjectCommandHandler(
    IProjectRepository projectRepository,
    IProjectRoleRepository projectRoleRepository,
    IProjectMemberRepository projectMemberRepository,
    IUserReadModelRepository userReadModelRepository,
    IOrganizationReadModelRepository organizationReadModelRepository,
    IUserContext userContext)
    : ICommandHandler<CreateProjectCommand, Result<ProjectResponse>>
{
    public async Task<Result<ProjectResponse>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        (IProjectOwner owner, OwnerType ownerType, Guid? organizationId) = await ResolveOwnerAsync(request, cancellationToken);
        if (owner is null)
        {
            return Result.Failure<ProjectResponse>(ProjectErrors.OwnerNotFound);
        }

        bool abbreviationExists = await projectRepository.ExistsByAbbreviationAsync(request.Abbreviation, owner.Id, cancellationToken: cancellationToken);
        if (abbreviationExists)
        {
            return Result.Failure<ProjectResponse>(ProjectErrors.DuplicatedAbbreviation);
        }

        Project project = Project.Create(
            request.Name,
            request.Abbreviation,
            request.PrivacyLevel,
            owner,
            ownerType);

        await projectRepository.AddAsync(project, cancellationToken);
        await projectRepository.SaveChangesAsync(cancellationToken);

        ProjectRole ownerRole = await CreateOwnerRoleAsync(project, cancellationToken);
        await AddCreatorAsMemberAsync(project, ownerRole, cancellationToken);

        return Result.Success(ToResponse(project, organizationId));
    }

    private async Task<ProjectRole> CreateOwnerRoleAsync(Project project, CancellationToken cancellationToken)
    {
        List<ProjectPermission> allPermissions = await projectRoleRepository.GetAllPermissionsAsync(cancellationToken);

        ProjectRole ownerRole = new(Guid.NewGuid(), "Owner", project, isOwnerRole: true);
        await projectRoleRepository.AddAsync(ownerRole, cancellationToken);

        List<ProjectRolePermission> rolePermissions = allPermissions
            .Select(p => new ProjectRolePermission(Guid.NewGuid(), PermissionLevel.Full, p, ownerRole))
            .ToList();

        await projectRoleRepository.AddPermissionsAsync(rolePermissions, cancellationToken);
        await projectRoleRepository.SaveChangesAsync(cancellationToken);

        return ownerRole;
    }

    private async Task AddCreatorAsMemberAsync(Project project, ProjectRole ownerRole, CancellationToken cancellationToken)
    {
        ProjectMember member = new(Guid.NewGuid(), userContext.UserId, MemberType.User, ownerRole, project);
        await projectMemberRepository.AddAsync(member, cancellationToken);
        await projectMemberRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task<(IProjectOwner Owner, OwnerType OwnerType, Guid? OrganizationId)> ResolveOwnerAsync(
        CreateProjectCommand request,
        CancellationToken cancellationToken)
    {
        if (request.OrganizationId.HasValue)
        {
            OrganizationReadModel? organization = await organizationReadModelRepository.GetByIdAsync(request.OrganizationId.Value, cancellationToken);
            if (organization is null)
            {
                return (null!, default, null);
            }

            return (organization, OwnerType.Organization, organization.Id);
        }

        UserReadModel? user = await userReadModelRepository.GetByIdAsync(userContext.UserId, cancellationToken);
        if (user is null)
        {
            return (null!, default, null);
        }

        return (user, OwnerType.User, null);
    }

    private static ProjectResponse ToResponse(Project project, Guid? organizationId) =>
        new(
            project.Id,
            project.Name,
            project.Abbreviation,
            project.Privacy,
            organizationId,
            project.CreatedAt);
}
