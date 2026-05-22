using FluentAssertions;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Entities.Properties;

namespace Modules.WorkItems.Domains.UnitTests.WorkLogs;

public class WorkLogTests
{
    private static WorkLog CreateWorkLog(DateTime? date = null, decimal hours = 2m, string comment = "Initial work")
    {
        UserReadModel user = new(Guid.NewGuid(), "John", "Doe");
        ProjectReadModel project = new(Guid.NewGuid(), "TST");
        WorkItemStatus status = new(Guid.NewGuid(), "To Do");
        WorkItemType type = new(Guid.NewGuid(), "Task");
        WorkItem workItem = WorkItem.Create(1, "Title", "Desc", 3m, DateTime.UtcNow.AddDays(7), project, status, type);

        return new WorkLog(Guid.NewGuid(), date ?? DateTime.UtcNow, hours, comment, workItem, user);
    }

    [Fact]
    public void Update_Should_UpdateDate_When_Provided()
    {
        // Arrange
        WorkLog workLog = CreateWorkLog();
        DateTime newDate = new(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act
        workLog.Update(newDate, null, null);

        // Assert
        workLog.Date.Should().Be(newDate);
    }

    [Fact]
    public void Update_Should_UpdateHours_When_Provided()
    {
        // Arrange
        WorkLog workLog = CreateWorkLog(hours: 2m);

        // Act
        workLog.Update(null, 4.5m, null);

        // Assert
        workLog.Hours.Should().Be(4.5m);
    }

    [Fact]
    public void Update_Should_UpdateComment_When_Provided()
    {
        // Arrange
        WorkLog workLog = CreateWorkLog(comment: "Old comment");

        // Act
        workLog.Update(null, null, "New comment");

        // Assert
        workLog.Comment.Should().Be("New comment");
    }

    [Fact]
    public void Update_Should_NotChange_When_AllNullsProvided()
    {
        // Arrange
        DateTime originalDate = new(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc);
        WorkLog workLog = CreateWorkLog(date: originalDate, hours: 3m, comment: "Keep this");

        // Act
        workLog.Update(null, null, null);

        // Assert
        workLog.Date.Should().Be(originalDate);
        workLog.Hours.Should().Be(3m);
        workLog.Comment.Should().Be("Keep this");
    }

    [Fact]
    public void Update_Should_UpdateAllFields_When_AllProvided()
    {
        // Arrange
        WorkLog workLog = CreateWorkLog();
        DateTime newDate = new(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act
        workLog.Update(newDate, 8m, "Full day");

        // Assert
        workLog.Date.Should().Be(newDate);
        workLog.Hours.Should().Be(8m);
        workLog.Comment.Should().Be("Full day");
    }
}
