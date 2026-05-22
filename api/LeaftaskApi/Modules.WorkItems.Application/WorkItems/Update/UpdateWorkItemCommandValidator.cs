using FluentValidation;

namespace Modules.WorkItems.Application.WorkItems.Update;

public sealed class UpdateWorkItemCommandValidator : AbstractValidator<UpdateWorkItemCommand>
{
    public UpdateWorkItemCommandValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(255)
            .When(x => x.Title is not null);

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .When(x => x.Description is not null);

        RuleFor(x => x.Progress)
            .InclusiveBetween(0, 100)
            .When(x => x.Progress.HasValue);
    }
}
