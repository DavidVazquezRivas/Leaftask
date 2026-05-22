using FluentValidation;

namespace Modules.Projects.Application.Permissions.CreateRole;

public sealed class CreateProjectRoleCommandValidator : AbstractValidator<CreateProjectRoleCommand>
{
    public CreateProjectRoleCommandValidator()
    {
        RuleFor(c => c.ProjectId).NotEmpty();
        RuleFor(c => c.Name).NotEmpty().MaximumLength(100);
        RuleFor(c => c.Permissions).NotNull();
        RuleForEach(c => c.Permissions).ChildRules(input => input.RuleFor(p => p.PermissionId).NotEmpty());
    }
}
