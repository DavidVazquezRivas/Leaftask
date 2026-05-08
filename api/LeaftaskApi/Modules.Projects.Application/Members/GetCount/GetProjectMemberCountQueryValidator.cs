using FluentValidation;

namespace Modules.Projects.Application.Members.GetCount;

public sealed class GetProjectMemberCountQueryValidator : AbstractValidator<GetProjectMemberCountQuery>
{
    public GetProjectMemberCountQueryValidator()
    {
        RuleFor(q => q.ProjectId).NotEmpty();
    }
}
