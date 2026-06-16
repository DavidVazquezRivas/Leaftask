using System.ComponentModel;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Tools;
using MediatR;
using Microsoft.SemanticKernel;
using Modules.Projects.Application.Fields.CreateCustomField;
using Modules.Projects.Application.Fields.DeleteCustomField;
using Modules.Projects.Application.Fields.GetFieldTypes;
using Modules.Projects.Application.Fields.GetProjectCustomFields;
using Modules.Projects.Application.Fields.PatchCustomField;

namespace Modules.Projects.DrivingInfrastructure.Tools;

public class ProjectCustomFieldsAiTool(ISender sender, IAiResponseFormatter formatter) : IAiTool
{
    [KernelFunction("GetProjectCustomFieldTypes")]
    [Description(
        "Retrieves a list of all available data types for custom fields (e.g., text, number, date, dropdown). Useful to know the valid Type IDs (GUIDs) before creating or modifying a custom field.")]
    public async Task<string> GetFieldTypesAsync(CancellationToken cancellationToken = default)
    {
        Result<IReadOnlyList<FieldTypeDto>> result = await sender.Send(new GetFieldTypesQuery(), cancellationToken);

        if (result.IsFailure)
        {
            return formatter.FormatFailure(nameof(GetFieldTypesAsync), result.Error.Description);
        }

        return formatter.FormatResponse(result.Value);
    }

    [KernelFunction("GetProjectCustomFields")]
    [Description(
        "Retrieves all custom fields currently defined for a specific project. Useful to view what extra metadata is being tracked and to find specific Field IDs.")]
    public async Task<string> GetCustomFieldsAsync(
        [Description(
            "The unique identifier (GUID) of the project. If you only have the project name, resolve it first using 'GetOrganizationProjects' or 'GetMyProjects'.")]
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        Result<IReadOnlyList<CustomFieldDto>> result =
            await sender.Send(new GetProjectCustomFieldsQuery(projectId), cancellationToken);

        if (result.IsFailure)
        {
            return formatter.FormatFailure(nameof(GetCustomFieldsAsync), result.Error.Description);
        }

        return formatter.FormatResponse(result.Value);
    }

    [KernelFunction("CreateProjectCustomField")]
    [Description(
        "Creates a new custom field for a project, allowing the team to track additional data on their entities.")]
    public async Task<string> CreateCustomFieldAsync(
        [Description("The unique identifier (GUID) of the project.")]
        Guid projectId,
        [Description("The name of the new custom field (e.g., 'Client Name', 'Estimated Budget').")]
        string name,
        [Description(
            "The unique identifier (GUID) of the data type. You MUST call 'GetProjectCustomFieldTypes' first to know valid type IDs if you are unsure.")]
        Guid type,
        [Description(
            "A list of predefined options for the field. Only applicable if the field type supports options (like a dropdown). Pass an empty array if not needed.")]
        IReadOnlyList<string> options,
        [Description("Whether this field is mandatory to fill out.")]
        bool required,
        [Description(
            "A list of unique identifiers (GUIDs) representing the target entities this field applies to (e.g., specific node types or tasks).")]
        IReadOnlyList<Guid> appliesTo,
        CancellationToken cancellationToken = default)
    {
        CreateCustomFieldCommand command = new(
            projectId,
            name,
            type,
            options,
            required,
            appliesTo);

        Result<CustomFieldDto> result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return formatter.FormatFailure(nameof(CreateCustomFieldAsync), result.Error.Description);
        }

        return formatter.FormatResponse(result.Value);
    }

    [KernelFunction("UpdateProjectCustomField")]
    [Description("Updates the properties (name, type, options, etc.) of an existing custom field within a project.")]
    public async Task<string> PatchCustomFieldAsync(
        [Description("The unique identifier (GUID) of the project.")]
        Guid projectId,
        [Description(
            "The unique identifier (GUID) of the custom field to update. Find this ID first using 'GetProjectCustomFields'.")]
        Guid fieldId,
        [Description("The new name of the custom field. Leave as null to keep current.")]
        string? name = null,
        [Description("The new data type (GUID). Leave as null to keep current.")]
        Guid? type = null,
        [Description("The new list of options. Leave as null to keep current.")]
        IReadOnlyList<string>? options = null,
        [Description("Whether this field is required. Leave as null to keep current.")]
        bool? required = null,
        [Description("The new list of target entities (GUIDs) this applies to. Leave as null to keep current.")]
        IReadOnlyList<Guid>? appliesTo = null,
        CancellationToken cancellationToken = default)
    {
        PatchCustomFieldCommand command = new(
            projectId,
            fieldId,
            name,
            type,
            options,
            required,
            appliesTo);

        Result<CustomFieldDto> result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return formatter.FormatFailure(nameof(PatchCustomFieldAsync), result.Error.Description);
        }

        return formatter.FormatResponse(result.Value);
    }

    [KernelFunction("DeleteProjectCustomField")]
    [Description(
        "Permanently deletes a custom field from a project. This might affect previously stored data inside tasks that used this field.")]
    public async Task<string> DeleteCustomFieldAsync(
        [Description("The unique identifier (GUID) of the project.")]
        Guid projectId,
        [Description(
            "The unique identifier (GUID) of the custom field to be deleted. Find this ID first using 'GetProjectCustomFields'.")]
        Guid fieldId,
        CancellationToken cancellationToken = default)
    {
        DeleteCustomFieldCommand command = new(projectId, fieldId);

        Result result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return formatter.FormatFailure(nameof(DeleteCustomFieldAsync), result.Error.Description);
        }

        return formatter.FormatMessage("Project custom field deleted successfully.");
    }
}
