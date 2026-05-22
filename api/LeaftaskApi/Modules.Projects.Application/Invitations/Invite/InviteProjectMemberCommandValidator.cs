using FluentValidation;

namespace Modules.Projects.Application.Invitations.Invite;

public sealed class InviteProjectMemberCommandValidator : AbstractValidator<InviteProjectMemberCommand>
{
    public InviteProjectMemberCommandValidator()
    {
        RuleFor(c => c.ProjectId).NotEmpty();
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.RoleId).NotEmpty();
    }
}
