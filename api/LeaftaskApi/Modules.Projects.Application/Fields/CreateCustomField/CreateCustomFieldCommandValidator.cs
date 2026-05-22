using FluentValidation;

namespace Modules.Projects.Application.Fields.CreateCustomField;

public sealed class CreateCustomFieldCommandValidator : AbstractValidator<CreateCustomFieldCommand>
{
    public CreateCustomFieldCommandValidator()
    {
        RuleFor(c => c.ProjectId).NotEmpty();
        RuleFor(c => c.Name).NotEmpty().MaximumLength(100);
        RuleFor(c => c.TypeId).NotEmpty();
        RuleForEach(c => c.Options).NotEmpty().MaximumLength(100);
    }
}
