using AIDotNet.Toon;
using AIDotNet.Toon.Options;

namespace BuildingBlocks.DrivingInfrastructure.Tools;

public class ToonResponseFormatter : IAiResponseFormatter
{
    public string FormatFailure(string operationName, string errorMessage)
    {
        return $"[ERROR] Tool '{operationName}' failed. Reason: {errorMessage}";
    }

    public string FormatResponse<T>(IEnumerable<T> items)
    {
        if (!items.Any())
        {
            return "No results found.";
        }

        ToonEncodeOptions options = new()
        {
            Delimiter = ToonDelimiter.PIPE
        };

        return ToonEncoder.Encode(items, options);
    }

    public string FormatResponse<T>(T item)
    {
        ToonEncodeOptions options = new()
        {
            Delimiter = ToonDelimiter.PIPE
        };

        return ToonEncoder.Encode(item, options);
    }

    public string FormatMessage(string message)
    {
        return $"[SUCCESS] {message}";
    }
}
