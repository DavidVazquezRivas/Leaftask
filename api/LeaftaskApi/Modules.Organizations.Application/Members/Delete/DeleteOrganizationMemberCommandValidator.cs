using FluentValidation;

namespace Modules.Organizations.Application.Members.Delete;

public sealed class DeleteOrganizationMemberCommandValidator : AbstractValidator<DeleteOrganizationMemberCommand>
{
    public DeleteOrganizationMemberCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty();

        RuleFor(x => x.MemberId)
            .NotEmpty();
    }
}
