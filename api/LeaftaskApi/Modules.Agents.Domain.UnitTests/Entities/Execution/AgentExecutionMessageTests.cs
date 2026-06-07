using FluentAssertions;
using Modules.Agents.Domain.Entities.Execution;

namespace Modules.Agents.Domain.UnitTests.Entities.Execution;

public class AgentExecutionMessageTests
{
    [Fact]
    public void Create_Should_SetAllProperties()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        Guid executionId = Guid.NewGuid();
        const MessageRole role = MessageRole.User;
        const string content = "Please check the overdue tasks";
        const string toolCalls = """[{"tool":"GetWorkItems"}]""";
        const int sequence = 3;

        // Act
        AgentExecutionMessage message = new(id, executionId, role, content, toolCalls, sequence);

        // Assert
        message.Id.Should().Be(id);
        message.ExecutionId.Should().Be(executionId);
        message.Role.Should().Be(role);
        message.Content.Should().Be(content);
        message.ToolCalls.Should().Be(toolCalls);
        message.SequenceNumber.Should().Be(sequence);
    }

    [Fact]
    public void Create_Should_AcceptNullToolCalls()
    {
        // Act
        AgentExecutionMessage message = new(
            Guid.NewGuid(), Guid.NewGuid(), MessageRole.Assistant, "All tasks are on track.", null, 2);

        // Assert
        message.ToolCalls.Should().BeNull();
    }

    [Theory]
    [InlineData(MessageRole.System)]
    [InlineData(MessageRole.User)]
    [InlineData(MessageRole.Assistant)]
    [InlineData(MessageRole.Tool)]
    public void Create_Should_SupportAllRoles(MessageRole role)
    {
        // Act
        AgentExecutionMessage message = new(Guid.NewGuid(), Guid.NewGuid(), role, "content", null, 1);

        // Assert
        message.Role.Should().Be(role);
    }
}
