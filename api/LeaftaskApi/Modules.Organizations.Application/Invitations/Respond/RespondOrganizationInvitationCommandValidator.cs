using FluentValidation;

namespace Modules.Organizations.Application.Invitations.Respond;

public sealed class RespondOrganizationInvitationCommandValidator : AbstractValidator<RespondOrganizationInvitationCommand>
{
    private static readonly HashSet<string> AllowedStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "accepted",
        "rejected",
        "canceled"
    };

    public RespondOrganizationInvitationCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty();

        RuleFor(x => x.InvitationId)
            .NotEmpty();

        RuleFor(x => x.Status)
            .NotEmpty()
            .Must(status => AllowedStatuses.Contains(status))
            .WithMessage("The status must be accepted, rejected or canceled.");
    }
}
