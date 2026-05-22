using FluentValidation;

namespace Modules.Projects.Application.Permissions.GetRoles;

public sealed class GetProjectRolesQueryValidator : AbstractValidator<GetProjectRolesQuery>
{
    public GetProjectRolesQueryValidator()
    {
        RuleFor(q => q.ProjectId).NotEmpty();
    }
}
