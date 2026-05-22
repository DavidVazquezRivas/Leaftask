using FluentValidation;

namespace Modules.Projects.Application.Permissions.GetPermissions;

public sealed class GetProjectPermissionsQueryValidator : AbstractValidator<GetProjectPermissionsQuery>
{
    public GetProjectPermissionsQueryValidator()
    {
        RuleFor(q => q.ProjectId).NotEmpty();
    }
}
