using FluentValidation;

namespace Modules.Projects.Application.Members.Remove;

public sealed class RemoveProjectMemberCommandValidator : AbstractValidator<RemoveProjectMemberCommand>
{
    public RemoveProjectMemberCommandValidator()
    {
        RuleFor(c => c.ProjectId).NotEmpty();
        RuleFor(c => c.MemberId).NotEmpty();
    }
}
