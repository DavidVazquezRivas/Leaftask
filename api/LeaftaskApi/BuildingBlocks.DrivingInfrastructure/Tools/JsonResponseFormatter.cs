using System.Text.Json;
using System.Text.Json.Serialization;

namespace BuildingBlocks.DrivingInfrastructure.Tools;

public sealed class JsonResponseFormatter : IAiResponseFormatter
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    public string FormatFailure(string operationName, string errorMessage)
        => $"[ERROR] Tool '{operationName}' failed. Reason: {errorMessage}";

    public string FormatResponse<T>(IEnumerable<T> items)
    {
        List<T> list = items.ToList();
        return list.Count == 0
            ? "No results found."
            : JsonSerializer.Serialize(list, Options);
    }

    public string FormatResponse<T>(T item)
        => JsonSerializer.Serialize(item, Options);

    public string FormatMessage(string message)
        => $"[SUCCESS] {message}";
}
