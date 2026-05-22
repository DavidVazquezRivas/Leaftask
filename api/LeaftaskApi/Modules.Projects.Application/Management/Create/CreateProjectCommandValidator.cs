using FluentValidation;

namespace Modules.Projects.Application.Management.Create;

public sealed class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(command => command.Abbreviation)
            .NotEmpty()
            .MaximumLength(3);

        RuleFor(command => command.PrivacyLevel)
            .IsInEnum();
    }
}
