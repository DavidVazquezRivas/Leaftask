using FluentValidation;

namespace Modules.Notification.Application.ApprovalRequests.UpdateStatus;

public sealed class UpdateApprovalStatusCommandValidator : AbstractValidator<UpdateApprovalStatusCommand>
{
    private static readonly string[] AllowedStatuses = ["approved", "rejected"];

    public UpdateApprovalStatusCommandValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty()
            .Must(s => AllowedStatuses.Contains(s, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Status must be 'approved' or 'rejected'.");
    }
}
