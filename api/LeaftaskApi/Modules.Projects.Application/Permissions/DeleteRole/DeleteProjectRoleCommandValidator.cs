using FluentValidation;

namespace Modules.Projects.Application.Permissions.DeleteRole;

public sealed class DeleteProjectRoleCommandValidator : AbstractValidator<DeleteProjectRoleCommand>
{
    public DeleteProjectRoleCommandValidator()
    {
        RuleFor(c => c.ProjectId).NotEmpty();
        RuleFor(c => c.RoleId).NotEmpty();
    }
}
