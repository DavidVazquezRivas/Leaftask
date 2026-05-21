using FluentValidation;

namespace Modules.WorkItems.Application.Comments.Add;

public sealed class AddCommentCommandValidator : AbstractValidator<AddCommentCommand>
{
    public AddCommentCommandValidator()
    {
        RuleFor(x => x.Content).NotEmpty().MaximumLength(20000);
    }
}
