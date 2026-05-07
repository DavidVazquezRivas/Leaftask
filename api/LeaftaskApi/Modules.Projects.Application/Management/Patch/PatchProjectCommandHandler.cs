using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Projects.Application.Authorization;
using Modules.Projects.Application.Management.Create;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Entities.Owner;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;

namespace Modules.Projects.Application.Management.Patch;

public sealed class PatchProjectCommandHandler(
    IProjectRepository projectRepository,
    IOrganizationPermissionChecker organizationPermissionChecker,
    IUserContext userContext)
    : ICommandHandler<PatchProjectCommand, Result<ProjectResponse>>
{
    public async Task<Result<ProjectResponse>> Handle(PatchProjectCommand command, CancellationToken cancellationToken)
    {
        Project? project = await projectRepository.GetByIdTrackedAsync(command.ProjectId, cancellationToken);
        if (project is null)
        {
            return Result.Failure<ProjectResponse>(ProjectErrors.ProjectNotFound);
        }

        bool canAccess = await CanAccessAsync(project, userContext.UserId, cancellationToken);
        if (!canAccess)
        {
            return Result.Failure<ProjectResponse>(ProjectErrors.AccessDenied);
        }

        if (command.Abbreviation is not null && command.Abbreviation != project.Abbreviation)
        {
            bool abbreviationTaken = await projectRepository.ExistsByAbbreviationAsync(
                command.Abbreviation, project.OwnerId, project.Id, cancellationToken);

            if (abbreviationTaken)
            {
                return Result.Failure<ProjectResponse>(ProjectErrors.DuplicatedAbbreviation);
            }
        }

        project.Update(command.Name, command.Abbreviation, command.PrivacyLevel);
        await projectRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(ToResponse(project));
    }

    private async Task<bool> CanAccessAsync(Project project, Guid userId, CancellationToken cancellationToken)
    {
        if (project.OwnerType == OwnerType.Organization)
        {
            return await organizationPermissionChecker.IsMemberAsync(project.OwnerId, userId, cancellationToken);
        }

        return project.OwnerId == userId;
    }

    private static ProjectResponse ToResponse(Project project)
    {
        Guid? organizationId = project.OwnerType == OwnerType.Organization ? project.OwnerId : null;

        return new ProjectResponse(
            project.Id,
            project.Name,
            project.Abbreviation,
            project.Privacy,
            organizationId,
            project.CreatedAt);
    }
}
