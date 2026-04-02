using BuildingBlocks.DrivingInfrastructure.Responses.Meta;

namespace BuildingBlocks.DrivingInfrastructure.Responses;

public class ApiResponse<T>
{
    public T Data { get; init; }
    public required ApiMeta Meta { get; init; }

    public static ApiResponse<T> Success(T data, ApiMeta meta) =>
        new() { Data = data, Meta = meta };
}

public class ApiResponse
{
    public ErrorDetails? Error { get; init; }
    public required ApiMeta Meta { get; init; }

    public static ApiResponse Failure(ErrorDetails error, ApiMeta meta) =>
        new() { Error = error, Meta = meta };
}