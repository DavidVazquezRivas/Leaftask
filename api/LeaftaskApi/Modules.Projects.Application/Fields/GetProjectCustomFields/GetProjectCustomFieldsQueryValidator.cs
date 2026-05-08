using FluentValidation;

namespace Modules.Projects.Application.Fields.GetProjectCustomFields;

public sealed class GetProjectCustomFieldsQueryValidator : AbstractValidator<GetProjectCustomFieldsQuery>
{
    public GetProjectCustomFieldsQueryValidator()
    {
        RuleFor(q => q.ProjectId).NotEmpty();
    }
}
