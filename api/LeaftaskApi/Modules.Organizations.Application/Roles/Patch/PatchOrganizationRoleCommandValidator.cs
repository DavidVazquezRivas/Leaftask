using FluentValidation;

namespace Modules.Organizations.Application.Roles.Patch;

public sealed class PatchOrganizationRoleCommandValidator : AbstractValidator<PatchOrganizationRoleCommand>
{
    public PatchOrganizationRoleCommandValidator()
    {
        RuleFor(x => x)
            .Must(command => command.Name is not null || command.Permissions is not null && command.Permissions.Count > 0)
            .WithMessage("At least one field must be provided.");

        RuleFor(x => x.Name)
            .MaximumLength(100)
            .When(x => x.Name is not null);

        RuleFor(x => x.Permissions)
            .Must(HaveUniquePermissionIds)
            .WithMessage("Permissions must be unique.")
            .When(x => x.Permissions is not null);
    }

    private static bool HaveUniquePermissionIds(IReadOnlyCollection<PatchOrganizationRolePermissionInput>? permissions) =>
        permissions is null || permissions.Select(permission => permission.OrganizationPermissionId).Distinct().Count() == permissions.Count;
}
