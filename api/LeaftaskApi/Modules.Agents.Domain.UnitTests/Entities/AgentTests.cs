using FluentAssertions;
using Modules.Agents.Domain.Entities;
using Modules.Agents.Domain.UnitTests.TestBuilders;

namespace Modules.Agents.Domain.UnitTests.Entities;

public class AgentTests
{
    [Fact]
    public void Create_Should_SetBasicProperties_Correctly()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        const string name = "Daily Standup Agent";
        const string instructions = "Check overdue tasks every morning and summarize them";
        const string systemPrompt = "You are a helpful project management assistant";

        // Act
        Agent agent = AgentTestBuilder.AnAgent()
            .WithProjectId(projectId)
            .WithName(name)
            .WithInstructions(instructions)
            .WithSystemPrompt(systemPrompt)
            .Build();

        // Assert
        agent.Id.Should().NotBe(Guid.Empty);
        agent.ProjectId.Should().Be(projectId);
        agent.Name.Should().Be(name);
        agent.Instructions.Should().Be(instructions);
        agent.SystemPrompt.Should().Be(systemPrompt);
        agent.TemplateId.Should().BeNull();
        agent.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Create_Should_SetTemplateId_When_Provided()
    {
        // Arrange
        Guid templateId = Guid.NewGuid();

        // Act
        Agent agent = AgentTestBuilder.AnAgent()
            .WithTemplateId(templateId)
            .Build();

        // Assert
        agent.TemplateId.Should().Be(templateId);
    }

    [Fact]
    public void Create_Should_HaveEmptyTriggers_When_NoneProvided()
    {
        // Act
        Agent agent = AgentTestBuilder.AnAgent().Build();

        // Assert
        agent.EventTriggers.Should().BeEmpty();
        agent.TimeTriggers.Should().BeEmpty();
    }

    [Fact]
    public void Create_Should_AddEventTriggers_When_Provided()
    {
        // Act
        Agent agent = AgentTestBuilder.AnAgent()
            .WithEventTrigger("workitem.created", "A new work item was created in your project: {payload}")
            .WithEventTrigger("workitem.status_changed", "A work item status changed: {payload}")
            .Build();

        // Assert
        agent.EventTriggers.Should().HaveCount(2);
        agent.EventTriggers.Select(t => t.Event).Should().Contain("workitem.created");
        agent.EventTriggers.Select(t => t.Event).Should().Contain("workitem.status_changed");
        agent.EventTriggers.Select(t => t.UserPrompt).Should().Contain("A new work item was created in your project: {payload}");
    }

    [Fact]
    public void Create_Should_AddTimeTriggers_When_Provided()
    {
        // Act
        Agent agent = AgentTestBuilder.AnAgent()
            .WithTimeTrigger("Daily morning check", "0 9 * * *", "Europe/Madrid")
            .Build();

        // Assert
        agent.TimeTriggers.Should().HaveCount(1);
        agent.TimeTriggers[0].Name.Should().Be("Daily morning check");
        agent.TimeTriggers[0].CronExpression.Should().Be("0 9 * * *");
        agent.TimeTriggers[0].TimeZone.Should().Be("Europe/Madrid");
        agent.TimeTriggers[0].IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_Should_AddMultipleTimeTriggers()
    {
        // Act
        Agent agent = AgentTestBuilder.AnAgent()
            .WithTimeTrigger("Morning", "0 9 * * *")
            .WithTimeTrigger("Evening", "0 18 * * *")
            .WithTimeTrigger("Every 10 minutes", "*/10 * * * *")
            .Build();

        // Assert
        agent.TimeTriggers.Should().HaveCount(3);
    }

    [Fact]
    public void Create_Should_AssignUniqueIds_ToEachEventTrigger()
    {
        // Act
        Agent agent = AgentTestBuilder.AnAgent()
            .WithEventTrigger("workitem.created", "New item")
            .WithEventTrigger("chat.message_sent", "New message")
            .WithEventTrigger("workitem.deleted", "Item deleted")
            .Build();

        // Assert
        agent.EventTriggers.Select(t => t.Id).Should().OnlyHaveUniqueItems();
        agent.EventTriggers.Select(t => t.Id).Should().NotContain(Guid.Empty);
    }

    [Fact]
    public void Create_Should_AssignUniqueIds_ToEachTimeTrigger()
    {
        // Act
        Agent agent = AgentTestBuilder.AnAgent()
            .WithTimeTrigger("Morning")
            .WithTimeTrigger("Evening")
            .Build();

        // Assert
        agent.TimeTriggers.Select(t => t.Id).Should().OnlyHaveUniqueItems();
        agent.TimeTriggers.Select(t => t.Id).Should().NotContain(Guid.Empty);
    }

    [Fact]
    public void Create_Should_SetModelConfig_Correctly()
    {
        // Act
        Agent agent = AgentTestBuilder.AnAgent().Build();

        // Assert
        agent.ModelConfig.Should().NotBeNull();
        agent.ModelConfig.Model.Should().NotBeNull();
        agent.ModelConfig.Model.Provider.Should().NotBeNull();
        agent.ModelConfig.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Create_Should_UseProvidedId()
    {
        // Arrange
        Guid agentId = Guid.NewGuid();

        // Act
        Agent agent = AgentTestBuilder.AnAgent().WithId(agentId).Build();

        // Assert
        agent.Id.Should().Be(agentId);
    }

    [Fact]
    public void Create_Should_SupportBothEventAndTimeTriggers_Together()
    {
        // Act
        Agent agent = AgentTestBuilder.AnAgent()
            .WithEventTrigger("workitem.created", "New item created")
            .WithTimeTrigger("Daily report", "0 8 * * *")
            .Build();

        // Assert
        agent.EventTriggers.Should().HaveCount(1);
        agent.TimeTriggers.Should().HaveCount(1);
    }
}
