namespace BuildingBlocks.Domain.Result;

public record Error(string Code, string Description, int StatusCode)
{
    public static readonly Error None = new(string.Empty, string.Empty, 200);
    public static readonly Error NullValue = new("General.Null", "Null value was provided", 500);
}
