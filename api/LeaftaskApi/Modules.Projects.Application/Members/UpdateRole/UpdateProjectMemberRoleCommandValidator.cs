using FluentValidation;

namespace Modules.Projects.Application.Members.UpdateRole;

public sealed class UpdateProjectMemberRoleCommandValidator : AbstractValidator<UpdateProjectMemberRoleCommand>
{
    public UpdateProjectMemberRoleCommandValidator()
    {
        RuleFor(c => c.ProjectId).NotEmpty();
        RuleFor(c => c.MemberId).NotEmpty();
        RuleFor(c => c.RoleId).NotEmpty();
    }
}
