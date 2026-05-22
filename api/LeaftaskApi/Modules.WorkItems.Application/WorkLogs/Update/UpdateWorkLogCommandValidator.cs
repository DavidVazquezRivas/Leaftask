using BuildingBlocks.Application.Commands;
using FluentValidation;

namespace Modules.WorkItems.Application.WorkLogs.Update;

public sealed class UpdateWorkLogCommandValidator : AbstractValidator<UpdateWorkLogCommand>
{
    public UpdateWorkLogCommandValidator()
    {
        When(x => x.Dedication.HasValue, () =>
            RuleFor(x => x.Dedication!.Value).GreaterThan(0).LessThanOrEqualTo(24));

        When(x => x.Description is not null, () =>
            RuleFor(x => x.Description!).NotEmpty().MaximumLength(1000));
    }
}
