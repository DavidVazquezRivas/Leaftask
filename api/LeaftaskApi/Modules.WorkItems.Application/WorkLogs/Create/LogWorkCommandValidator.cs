using BuildingBlocks.Application.Commands;
using FluentValidation;

namespace Modules.WorkItems.Application.WorkLogs.Create;

public sealed class LogWorkCommandValidator : AbstractValidator<LogWorkCommand>
{
    public LogWorkCommandValidator()
    {
        RuleFor(x => x.Dedication).GreaterThan(0).LessThanOrEqualTo(24);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
    }
}
