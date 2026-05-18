using BuildingBlocks.Domain.Result;
using FluentValidation;
using MediatR;

namespace Modules.WorkItems.Application.WorkItems.Create;

public sealed class CreateWorkItemCommandValidator : AbstractValidator<CreateWorkItemCommand>
{
    public CreateWorkItemCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.Description)
            .MaximumLength(2000);

        RuleFor(x => x.TypeId)
            .NotEmpty();

        RuleFor(x => x.StatusId)
            .NotEmpty();
    }
}
