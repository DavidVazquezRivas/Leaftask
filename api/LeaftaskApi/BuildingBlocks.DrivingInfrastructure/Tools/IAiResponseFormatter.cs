namespace BuildingBlocks.DrivingInfrastructure.Tools;

public interface IAiResponseFormatter
{
    string FormatFailure(string operationName, string errorMessage);

    string FormatResponse<T>(IEnumerable<T> items);

    string FormatResponse<T>(T item);

    string FormatMessage(string message);
}
