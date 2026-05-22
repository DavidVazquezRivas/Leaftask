using FluentValidation;

namespace Modules.Projects.Application.Management.Patch;

public sealed class PatchProjectCommandValidator : AbstractValidator<PatchProjectCommand>
{
    public PatchProjectCommandValidator()
    {
        RuleFor(command => command.ProjectId)
            .NotEmpty();

        RuleFor(command => command.Name)
            .MaximumLength(100)
            .When(command => command.Name is not null);

        RuleFor(command => command.Abbreviation)
            .MaximumLength(20)
            .When(command => command.Abbreviation is not null);
    }
}
