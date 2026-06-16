using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Agents.Application.Bootstrap;
using Modules.Agents.Domain;
using Modules.Agents.Domain.Entities;
using Modules.Agents.Domain.Entities.AgentTriggers;
using Modules.Agents.Domain.Entities.Model;
using Modules.Agents.Domain.Errors;
using Modules.Agents.Domain.Repositories;

namespace Modules.Agents.Application.Agents.Create;

public sealed class CreateAgentCommandHandler(
    IProjectReadModelRepository projectReadModelRepository,
    IModelRepository modelRepository,
    IAgentRepository agentRepository,
    IBootstrapAgentService bootstrapService,
    IAgentScheduler agentScheduler)
    : ICommandHandler<CreateAgentCommand, Result<AgentDto>>
{
    private static readonly IReadOnlyList<string> AvailableEventTypes =
    [
        AgentEventTypes.WorkItemCreated,
        AgentEventTypes.WorkItemStatusChanged,
        AgentEventTypes.WorkItemAssigneeChanged,
        AgentEventTypes.WorkItemDeleted,
        AgentEventTypes.WorkItemProgressUpdated,
        AgentEventTypes.WorkItemCommentAdded,
        AgentEventTypes.WorkItemMention,
        AgentEventTypes.ChatMessageSent,
    ];

    public async Task<Result<AgentDto>> Handle(CreateAgentCommand command, CancellationToken cancellationToken)
    {
        ProjectReadModel? project = await projectReadModelRepository.GetByIdAsync(command.ProjectId, cancellationToken);
        if (project is null)
            return Result.Failure<AgentDto>(AgentErrors.ProjectNotFound);

        IReadOnlyList<Model> models = await modelRepository.GetAllAsync(cancellationToken);
        if (models.Count == 0)
            return Result.Failure<AgentDto>(AgentErrors.ModelNotFound);

        IReadOnlyList<AvailableModelDto> availableModels = models
            .Select(m => new AvailableModelDto(m.Id, m.Name, m.Cost))
            .ToList();

        BootstrapAgentResult? bootstrapResult = await bootstrapService.GenerateAsync(
            command.Instructions,
            availableModels,
            AvailableEventTypes,
            cancellationToken);

        if (bootstrapResult is null)
            return Result.Failure<AgentDto>(AgentErrors.BootstrapFailed);

        Model selectedModel = models.FirstOrDefault(m => m.Id == bootstrapResult.ModelId)
                              ?? models.OrderBy(m => m.Cost).First();

        ModelConfig modelConfig = new(
            Guid.NewGuid(),
            selectedModel,
            bootstrapResult.Temperature,
            bootstrapResult.MaxTokens);

        DateTime now = DateTime.UtcNow;

        Agent agent = Agent.Create(
            Guid.NewGuid(),
            command.ProjectId,
            command.Name,
            command.Instructions,
            bootstrapResult.SystemPrompt,
            modelConfig,
            command.TemplateId,
            now,
            command.RoleId,
            bootstrapResult.EventTriggers.Select(e => (e.EventType, e.UserPrompt)),
            bootstrapResult.TimeTriggers.Select(t => (t.Name, t.CronExpression, t.TimeZone)));

        await agentRepository.AddAsync(agent, cancellationToken);
        await agentRepository.SaveChangesAsync(cancellationToken);

        foreach (AgentTimeTrigger timeTrigger in agent.TimeTriggers)
        {
            await agentScheduler.ScheduleTimeTriggerAsync(
                agent.Id,
                timeTrigger.Id,
                timeTrigger.CronExpression,
                timeTrigger.TimeZone,
                cancellationToken);
        }

        return Result.Success(new AgentDto(
            agent.Id,
            agent.ProjectId,
            agent.Name,
            agent.Instructions,
            agent.TemplateId,
            agent.CreatedAt));
    }
}
