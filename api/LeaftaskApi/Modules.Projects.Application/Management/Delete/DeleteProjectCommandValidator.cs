using FluentValidation;

namespace Modules.Projects.Application.Management.Delete;

public sealed class DeleteProjectCommandValidator : AbstractValidator<DeleteProjectCommand>
{
    public DeleteProjectCommandValidator()
    {
        RuleFor(command => command.ProjectId)
            .NotEmpty();
    }
}
