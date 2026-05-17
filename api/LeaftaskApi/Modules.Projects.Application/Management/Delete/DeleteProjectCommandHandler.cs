using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Projects.Application.Authorization;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Entities.Owner;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;

namespace Modules.Projects.Application.Management.Delete;

public sealed class DeleteProjectCommandHandler(
    IProjectRepository projectRepository,
    IOrganizationPermissionChecker organizationPermissionChecker,
    IUserContext userContext)
    : ICommandHandler<DeleteProjectCommand, Result>
{
    public async Task<Result> Handle(DeleteProjectCommand command, CancellationToken cancellationToken)
    {
        Project? project = await projectRepository.GetByIdTrackedAsync(command.ProjectId, cancellationToken);
        if (project is null)
        {
            return Result.Failure(ProjectErrors.ProjectNotFound);
        }

        Result accessResult = await CheckDeleteAccessAsync(project, userContext.UserId, cancellationToken);
        if (accessResult.IsFailure)
        {
            return accessResult;
        }

        project.Delete();
        await projectRepository.RemoveAsync(project, cancellationToken);
        await projectRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private async Task<Result> CheckDeleteAccessAsync(Project project, Guid userId, CancellationToken cancellationToken)
    {
        if (project.OwnerType == OwnerType.Organization)
        {
            OrganizationPermissionCheckStatus status = await organizationPermissionChecker.CheckAsync(
                project.OwnerId,
                userId,
                "Delete Projects",
                cancellationToken);

            return status switch
            {
                OrganizationPermissionCheckStatus.Full => Result.Success(),
                OrganizationPermissionCheckStatus.Supervised => Result.Failure(ProjectErrors.OrganizationPermissionApprovalRequired),
                OrganizationPermissionCheckStatus.Denied => Result.Failure(ProjectErrors.OrganizationPermissionDenied),
                OrganizationPermissionCheckStatus.MembershipRequired => Result.Failure(ProjectErrors.OrganizationMembershipRequired),
                OrganizationPermissionCheckStatus.PermissionNotFound => Result.Failure(ProjectErrors.OrganizationPermissionNotFound),
                OrganizationPermissionCheckStatus.OrganizationNotFound => Result.Failure(ProjectErrors.OrganizationNotFound),
                _ => Result.Failure(ProjectErrors.OrganizationPermissionDenied)
            };
        }

        return project.OwnerId == userId
            ? Result.Success()
            : Result.Failure(ProjectErrors.AccessDenied);
    }
}
