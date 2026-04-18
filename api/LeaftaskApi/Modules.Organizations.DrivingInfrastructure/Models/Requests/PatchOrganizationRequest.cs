namespace Modules.Organizations.DrivingInfrastructure.Models.Requests;

public record PatchOrganizationRequest(string? Name, string? Description, string? Website);
