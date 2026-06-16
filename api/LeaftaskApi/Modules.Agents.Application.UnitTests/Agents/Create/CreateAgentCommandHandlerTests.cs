using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Agents.Application.Agents;
using Modules.Agents.Application.Agents.Create;
using Modules.Agents.Application.Bootstrap;
using Modules.Agents.Application.UnitTests.TestBuilders;
using Modules.Agents.Domain.Entities;
using Modules.Agents.Domain.Entities.Model;
using Modules.Agents.Domain.Errors;
using Modules.Agents.Domain.Repositories;
using NSubstitute;

namespace Modules.Agents.Application.UnitTests.Agents.Create;

public class CreateAgentCommandHandlerTests
{
    private readonly CreateAgentCommandHandler _handler;
    private readonly IProjectReadModelRepository _projectReadModelRepositoryMock;
    private readonly IModelRepository _modelRepositoryMock;
    private readonly IAgentRepository _agentRepositoryMock;
    private readonly IBootstrapAgentService _bootstrapServiceMock;
    private readonly IAgentScheduler _agentSchedulerMock;

    public CreateAgentCommandHandlerTests()
    {
        _projectReadModelRepositoryMock = Substitute.For<IProjectReadModelRepository>();
        _modelRepositoryMock = Substitute.For<IModelRepository>();
        _agentRepositoryMock = Substitute.For<IAgentRepository>();
        _bootstrapServiceMock = Substitute.For<IBootstrapAgentService>();
        _agentSchedulerMock = Substitute.For<IAgentScheduler>();

        _handler = new CreateAgentCommandHandler(
            _projectReadModelRepositoryMock,
            _modelRepositoryMock,
            _agentRepositoryMock,
            _bootstrapServiceMock,
            _agentSchedulerMock);
    }

    private static Model CreateModel(Guid? id = null, double cost = 0.2)
    {
        ModelProvider provider = new(Guid.NewGuid(), "OpenAI", string.Empty);
        return new(id ?? Guid.NewGuid(), "gpt-test", provider, "Test model", cost);
    }

    private static BootstrapAgentResult CreateBootstrapResult(Guid modelId, bool withTimeTrigger = false, bool withEventTrigger = false)
    {
        List<BootstrapTimeTrigger> timeTriggers = withTimeTrigger
            ? [new BootstrapTimeTrigger("Daily check", "0 9 * * *", "UTC")]
            : [];

        List<BootstrapEventTrigger> eventTriggers = withEventTrigger
            ? [new BootstrapEventTrigger("workitem.created", "A new work item was created")]
            : [];

        return new BootstrapAgentResult(
            SystemPrompt: "You are a helpful project assistant",
            ModelId: modelId,
            Temperature: 0.7,
            MaxTokens: 1024,
            TimeTriggers: timeTriggers,
            EventTriggers: eventTriggers);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_And_CreateAgent_When_HappyPath()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid modelId = Guid.NewGuid();

        CreateAgentCommand command = CreateAgentCommandTestBuilder.ACommand()
            .WithProjectId(projectId)
            .WithName("Work Monitor")
            .Build();

        ProjectReadModel project = new(projectId, "TST");
        Model model = CreateModel(modelId);

        _projectReadModelRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
            .Returns(project);
        _modelRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(new List<Model> { model });
        _bootstrapServiceMock.GenerateAsync(
                command.Instructions,
                Arg.Any<IReadOnlyList<AvailableModelDto>>(),
                Arg.Any<IReadOnlyList<string>>(),
                Arg.Any<CancellationToken>())
            .Returns(CreateBootstrapResult(modelId));

        // Act
        Result<AgentDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ProjectId.Should().Be(projectId);
        result.Value.Name.Should().Be("Work Monitor");
        result.Value.Id.Should().NotBe(Guid.Empty);

        await _agentRepositoryMock.Received(1).AddAsync(Arg.Any<Agent>(), Arg.Any<CancellationToken>());
        await _agentRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProjectNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        CreateAgentCommand command = CreateAgentCommandTestBuilder.ACommand()
            .WithProjectId(projectId)
            .Build();

        _projectReadModelRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
            .Returns((ProjectReadModel?)null);

        // Act
        Result<AgentDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AgentErrors.ProjectNotFound);
        await _agentRepositoryMock.DidNotReceive().AddAsync(Arg.Any<Agent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_NoModelsAvailable()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        CreateAgentCommand command = CreateAgentCommandTestBuilder.ACommand()
            .WithProjectId(projectId)
            .Build();

        _projectReadModelRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
            .Returns(new ProjectReadModel(projectId, "TST"));
        _modelRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(new List<Model>());

        // Act
        Result<AgentDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AgentErrors.ModelNotFound);
        await _bootstrapServiceMock.DidNotReceive().GenerateAsync(
            Arg.Any<string>(),
            Arg.Any<IReadOnlyList<AvailableModelDto>>(),
            Arg.Any<IReadOnlyList<string>>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_BootstrapServiceReturnsNull()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        CreateAgentCommand command = CreateAgentCommandTestBuilder.ACommand()
            .WithProjectId(projectId)
            .Build();

        _projectReadModelRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
            .Returns(new ProjectReadModel(projectId, "TST"));
        _modelRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(new List<Model> { CreateModel() });
        _bootstrapServiceMock.GenerateAsync(
                Arg.Any<string>(),
                Arg.Any<IReadOnlyList<AvailableModelDto>>(),
                Arg.Any<IReadOnlyList<string>>(),
                Arg.Any<CancellationToken>())
            .Returns((BootstrapAgentResult?)null);

        // Act
        Result<AgentDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AgentErrors.BootstrapFailed);
        await _agentRepositoryMock.DidNotReceive().AddAsync(Arg.Any<Agent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ScheduleTimeTriggers_When_BootstrapIncludesTimeTriggers()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid modelId = Guid.NewGuid();

        CreateAgentCommand command = CreateAgentCommandTestBuilder.ACommand()
            .WithProjectId(projectId)
            .Build();

        _projectReadModelRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
            .Returns(new ProjectReadModel(projectId, "TST"));
        _modelRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(new List<Model> { CreateModel(modelId) });
        _bootstrapServiceMock.GenerateAsync(
                Arg.Any<string>(),
                Arg.Any<IReadOnlyList<AvailableModelDto>>(),
                Arg.Any<IReadOnlyList<string>>(),
                Arg.Any<CancellationToken>())
            .Returns(CreateBootstrapResult(modelId, withTimeTrigger: true));

        // Act
        Result<AgentDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _agentSchedulerMock.Received(1).ScheduleTimeTriggerAsync(
            Arg.Any<Guid>(),
            Arg.Any<Guid>(),
            "0 9 * * *",
            "UTC",
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_NotScheduleAnything_When_NoTimeTriggers()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid modelId = Guid.NewGuid();

        CreateAgentCommand command = CreateAgentCommandTestBuilder.ACommand()
            .WithProjectId(projectId)
            .Build();

        _projectReadModelRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
            .Returns(new ProjectReadModel(projectId, "TST"));
        _modelRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(new List<Model> { CreateModel(modelId) });
        _bootstrapServiceMock.GenerateAsync(
                Arg.Any<string>(),
                Arg.Any<IReadOnlyList<AvailableModelDto>>(),
                Arg.Any<IReadOnlyList<string>>(),
                Arg.Any<CancellationToken>())
            .Returns(CreateBootstrapResult(modelId, withTimeTrigger: false));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _agentSchedulerMock.DidNotReceive().ScheduleTimeTriggerAsync(
            Arg.Any<Guid>(),
            Arg.Any<Guid>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_FallbackToCheapestModel_When_BootstrapModelIdNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid unknownModelId = Guid.NewGuid();

        CreateAgentCommand command = CreateAgentCommandTestBuilder.ACommand()
            .WithProjectId(projectId)
            .Build();

        Model cheap = CreateModel(cost: 0.1);
        Model expensive = CreateModel(cost: 5.0);

        _projectReadModelRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
            .Returns(new ProjectReadModel(projectId, "TST"));
        _modelRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(new List<Model> { expensive, cheap });
        _bootstrapServiceMock.GenerateAsync(
                Arg.Any<string>(),
                Arg.Any<IReadOnlyList<AvailableModelDto>>(),
                Arg.Any<IReadOnlyList<string>>(),
                Arg.Any<CancellationToken>())
            .Returns(CreateBootstrapResult(unknownModelId));

        Agent? savedAgent = null;
        await _agentRepositoryMock.AddAsync(
            Arg.Do<Agent>(a => savedAgent = a),
            Arg.Any<CancellationToken>());

        // Act
        Result<AgentDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        savedAgent.Should().NotBeNull();
        savedAgent!.ModelConfig.Model.Cost.Should().Be(0.1);
    }

    [Fact]
    public async Task Handle_Should_IncludeTemplateId_When_Provided()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid modelId = Guid.NewGuid();
        Guid templateId = Guid.NewGuid();

        CreateAgentCommand command = CreateAgentCommandTestBuilder.ACommand()
            .WithProjectId(projectId)
            .WithTemplateId(templateId)
            .Build();

        _projectReadModelRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
            .Returns(new ProjectReadModel(projectId, "TST"));
        _modelRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(new List<Model> { CreateModel(modelId) });
        _bootstrapServiceMock.GenerateAsync(
                Arg.Any<string>(),
                Arg.Any<IReadOnlyList<AvailableModelDto>>(),
                Arg.Any<IReadOnlyList<string>>(),
                Arg.Any<CancellationToken>())
            .Returns(CreateBootstrapResult(modelId));

        // Act
        Result<AgentDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TemplateId.Should().Be(templateId);
    }
}
