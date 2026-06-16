using BuildingBlocks.Application.Commands;
using Modules.Projects.Domain.Entities.Member;
using Modules.Projects.Domain.Entities.Role;
using Modules.Projects.Domain.Repositories;

namespace Modules.Projects.Application.Agents.Create;

public sealed class CreateAgentReadModelOnAgentCreatedCommandHandler(
    IAgentReadModelRepository agentReadModelRepository,
    IProjectRepository projectRepository,
    IProjectMemberRepository memberRepository,
    IProjectRoleRepository roleRepository)
    : ICommandHandler<CreateAgentReadModelOnAgentCreatedCommand>
{
    public async Task Handle(CreateAgentReadModelOnAgentCreatedCommand request, CancellationToken cancellationToken)
    {
        bool exists = await agentReadModelRepository.ExistsByIdAsync(request.AgentId, cancellationToken);
        if (!exists)
        {
            AgentReadModel model = new(request.AgentId, request.Name);
            await agentReadModelRepository.AddAsync(model, cancellationToken);
        }

        bool alreadyMember = await memberRepository.ExistsByMemberIdAsync(
            request.ProjectId, request.AgentId, cancellationToken);

        if (!alreadyMember)
        {
            Domain.Entities.Project? project = await projectRepository.GetByIdTrackedAsync(
                request.ProjectId, cancellationToken);

            ProjectRole? role = await roleRepository.GetByIdTrackedAsync(
                request.ProjectId, request.RoleId, cancellationToken);

            // Fall back to any non-owner role if the specified role is not found
            role ??= await roleRepository.GetDefaultMemberRoleAsync(request.ProjectId, cancellationToken);

            if (project is not null && role is not null)
            {
                ProjectMember member = new(Guid.NewGuid(), request.AgentId, MemberType.Agent, role, project);
                await memberRepository.AddAsync(member, cancellationToken);
            }
        }

        await agentReadModelRepository.SaveChangesAsync(cancellationToken);
    }
}
