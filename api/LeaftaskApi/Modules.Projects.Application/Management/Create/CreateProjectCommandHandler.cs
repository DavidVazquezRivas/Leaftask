using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Entities.Owner;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;

namespace Modules.Projects.Application.Management.Create;

public sealed class CreateProjectCommandHandler(
    IProjectRepository projectRepository,
    IUserReadModelRepository userReadModelRepository,
    IOrganizationReadModelRepository organizationReadModelRepository,
    IUserContext userContext)
    : ICommandHandler<CreateProjectCommand, Result<ProjectResponse>>
{
    public async Task<Result<ProjectResponse>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        if (!ProjectPrivacyLevels.TryMap(request.PrivacyLevelId, out ProjectPrivacy privacy))
        {
            return Result.Failure<ProjectResponse>(ProjectErrors.InvalidPrivacyLevel);
        }

        bool abbreviationExists = await projectRepository.ExistsByAbbreviationAsync(request.Abbreviation, cancellationToken);
        if (abbreviationExists)
        {
            return Result.Failure<ProjectResponse>(ProjectErrors.DuplicatedAbbreviation);
        }

        (IProjectOwner owner, OwnerType ownerType, Guid? organizationId) = await ResolveOwnerAsync(request, cancellationToken);
        if (owner is null)
        {
            return Result.Failure<ProjectResponse>(ProjectErrors.OwnerNotFound);
        }

        Project project = Project.Create(
            request.Name,
            request.Abbreviation,
            privacy,
            owner,
            ownerType);

        await projectRepository.AddAsync(project, cancellationToken);
        await projectRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(ToResponse(project, organizationId));
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
            ProjectPrivacyLevels.ToDto(project.Privacy),
            organizationId,
            project.CreatedAt);
}
