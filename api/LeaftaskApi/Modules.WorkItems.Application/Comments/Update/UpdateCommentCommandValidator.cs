using FluentValidation;

namespace Modules.WorkItems.Application.Comments.Update;

public sealed class UpdateCommentCommandValidator : AbstractValidator<UpdateCommentCommand>
{
    public UpdateCommentCommandValidator()
    {
        RuleFor(x => x).Must(x => x.Content is not null || x.AttachmentIds is not null)
            .WithMessage("At least one field must be provided.");
        RuleFor(x => x.Content).NotEmpty().MaximumLength(20000).When(x => x.Content is not null);
    }
}
