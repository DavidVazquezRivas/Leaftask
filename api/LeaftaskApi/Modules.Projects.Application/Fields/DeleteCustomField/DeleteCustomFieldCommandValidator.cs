using FluentValidation;

namespace Modules.Projects.Application.Fields.DeleteCustomField;

public sealed class DeleteCustomFieldCommandValidator : AbstractValidator<DeleteCustomFieldCommand>
{
    public DeleteCustomFieldCommandValidator()
    {
        RuleFor(c => c.ProjectId).NotEmpty();
        RuleFor(c => c.FieldId).NotEmpty();
    }
}
