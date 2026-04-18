using FluentValidation;

namespace Modules.Organizations.Application.Roles.Create;

public sealed class CreateOrganizationRoleCommandValidator : AbstractValidator<CreateOrganizationRoleCommand>
{
    public CreateOrganizationRoleCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Permissions)
            .Must(HaveUniquePermissionIds)
            .WithMessage("Permissions must be unique.");
    }

    private static bool HaveUniquePermissionIds(IReadOnlyCollection<CreateOrganizationRolePermissionInput>? permissions) =>
        permissions is null || permissions.Select(permission => permission.OrganizationPermissionId).Distinct().Count() == permissions.Count;
}
