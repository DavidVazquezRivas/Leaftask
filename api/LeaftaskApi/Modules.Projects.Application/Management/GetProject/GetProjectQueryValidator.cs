using FluentValidation;

namespace Modules.Projects.Application.Management.GetProject;

public sealed class GetProjectQueryValidator : AbstractValidator<GetProjectQuery>
{
    public GetProjectQueryValidator()
    {
        RuleFor(query => query.ProjectId)
            .NotEmpty();
    }
}
