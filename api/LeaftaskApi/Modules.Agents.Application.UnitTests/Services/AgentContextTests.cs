using FluentAssertions;
using Modules.Agents.Application.Services;

namespace Modules.Agents.Application.UnitTests.Services;

public class AgentExecutionContextTests
{
    [Fact]
    public void Activate_Should_SetActiveStateAndIds()
    {
        AgentExecutionContext context = new();
        Guid agentId = Guid.NewGuid();
        Guid projectId = Guid.NewGuid();

        context.Activate(agentId, projectId);

        context.IsActive.Should().BeTrue();
        context.AgentId.Should().Be(agentId);
        context.ProjectId.Should().Be(projectId);
    }

    [Fact]
    public void Default_Should_BeInactive()
    {
        AgentExecutionContext context = new();
        context.IsActive.Should().BeFalse();
        context.AgentId.Should().Be(Guid.Empty);
        context.ProjectId.Should().Be(Guid.Empty);
    }
}

public class AgentSuspensionContextTests
{
    [Fact]
    public void RequestSuspension_Should_SetSuspensionState()
    {
        AgentSuspensionContext context = new();
        string[] correlationIds = ["id1", "id2"];

        context.RequestSuspension("TestEvent", correlationIds);

        context.ShouldSuspend.Should().BeTrue();
        context.EventType.Should().Be("TestEvent");
        context.CorrelationIds.Should().Equal(correlationIds);
    }

    [Fact]
    public void Default_Should_NotSuspend()
    {
        AgentSuspensionContext context = new();
        context.ShouldSuspend.Should().BeFalse();
        context.EventType.Should().BeEmpty();
        context.CorrelationIds.Should().BeEmpty();
    }
}
