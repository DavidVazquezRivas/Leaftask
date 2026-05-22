using FluentValidation;

namespace Modules.Projects.Application.Invitations.GetPending;

public sealed class GetPendingProjectInvitationsQueryValidator : AbstractValidator<GetPendingProjectInvitationsQuery>
{
    public GetPendingProjectInvitationsQueryValidator()
    {
        RuleFor(q => q.ProjectId).NotEmpty();
    }
}
