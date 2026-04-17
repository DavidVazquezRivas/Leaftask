using BuildingBlocks.Application.Validation;
using BuildingBlocks.Domain.Result;
using FluentValidation;

namespace Modules.Organizations.Application.Management.Patch;

public sealed class PatchOrganizationCommandValidator : AbstractValidator<PatchOrganizationCommand>
{
    public PatchOrganizationCommandValidator()
    {
        RuleFor(x => x)
            .Must(command => command.Name is not null || command.Description is not null || command.Website is not null)
            .WithMessage("At least one field must be provided to update the organization.");

        RuleFor(x => x.Name)
            .MaximumLength(100)
            .When(x => x.Name is not null);

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => x.Description is not null);

        RuleFor(x => x.Website)
            .MaximumLength(200)
            .When(x => x.Website is not null);
    }
}
