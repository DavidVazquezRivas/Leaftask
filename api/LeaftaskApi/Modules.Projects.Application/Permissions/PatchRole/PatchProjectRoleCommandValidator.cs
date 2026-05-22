using FluentValidation;

namespace Modules.Projects.Application.Permissions.PatchRole;

public sealed class PatchProjectRoleCommandValidator : AbstractValidator<PatchProjectRoleCommand>
{
    public PatchProjectRoleCommandValidator()
    {
        RuleFor(c => c.ProjectId).NotEmpty();
        RuleFor(c => c.RoleId).NotEmpty();
        When(c => c.Name is not null, () => RuleFor(c => c.Name).NotEmpty().MaximumLength(100));
        RuleForEach(c => c.Permissions).ChildRules(input => input.RuleFor(p => p.PermissionId).NotEmpty());
    }
}
