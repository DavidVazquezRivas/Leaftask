using FluentValidation;

namespace Modules.Projects.Application.Fields.PatchCustomField;

public sealed class PatchCustomFieldCommandValidator : AbstractValidator<PatchCustomFieldCommand>
{
    public PatchCustomFieldCommandValidator()
    {
        RuleFor(c => c.ProjectId).NotEmpty();
        RuleFor(c => c.FieldId).NotEmpty();
        RuleFor(c => c.Name).MaximumLength(100).When(c => c.Name is not null);
        RuleForEach(c => c.Options).NotEmpty().MaximumLength(100).When(c => c.Options is not null);
    }
}
