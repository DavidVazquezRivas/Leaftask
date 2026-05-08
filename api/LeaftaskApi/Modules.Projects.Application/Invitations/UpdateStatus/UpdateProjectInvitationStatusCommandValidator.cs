using FluentValidation;

namespace Modules.Projects.Application.Invitations.UpdateStatus;

public sealed class UpdateProjectInvitationStatusCommandValidator
    : AbstractValidator<UpdateProjectInvitationStatusCommand>
{
    private static readonly HashSet<string> ValidStatuses =
        ["ACCEPTED", "REJECTED", "CANCELLED"];

    public UpdateProjectInvitationStatusCommandValidator()
    {
        RuleFor(c => c.ProjectId).NotEmpty();
        RuleFor(c => c.InvitationId).NotEmpty();
        RuleFor(c => c.Status)
            .NotEmpty()
            .Must(s => ValidStatuses.Contains(s.ToUpperInvariant()))
            .WithMessage("Status must be one of: accepted, rejected, cancelled.");
    }
}
