using FluentValidation;

namespace Modules.Notification.Application.ApprovalRequests.AddComment;

public sealed class AddApprovalCommentCommandValidator : AbstractValidator<AddApprovalCommentCommand>
{
    public AddApprovalCommentCommandValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(2000);
    }
}
