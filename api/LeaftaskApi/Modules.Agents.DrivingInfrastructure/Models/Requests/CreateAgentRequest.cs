namespace Modules.Agents.DrivingInfrastructure.Models.Requests;

public sealed record CreateAgentRequest(
    Guid? ProjectId,
    string Name,
    string Instructions,
    string? TemplateId);
