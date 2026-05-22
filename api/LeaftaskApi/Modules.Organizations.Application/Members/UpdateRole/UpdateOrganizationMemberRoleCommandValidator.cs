using FluentValidation;

namespace Modules.Organizations.Application.Members.UpdateRole;

public sealed class UpdateOrganizationMemberRoleCommandValidator : AbstractValidator<UpdateOrganizationMemberRoleCommand>
{
    public UpdateOrganizationMemberRoleCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty();

        RuleFor(x => x.MemberId)
            .NotEmpty();

        RuleFor(x => x.RoleId)
            .NotEmpty();
    }
}
