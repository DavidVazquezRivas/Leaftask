using FluentValidation;

namespace Modules.Projects.Application.Members.GetMembers;

public sealed class GetProjectMembersQueryValidator : AbstractValidator<GetProjectMembersQuery>
{
    public GetProjectMembersQueryValidator()
    {
        RuleFor(q => q.ProjectId).NotEmpty();
        RuleFor(q => q.Limit).InclusiveBetween(1, 100);
    }
}
