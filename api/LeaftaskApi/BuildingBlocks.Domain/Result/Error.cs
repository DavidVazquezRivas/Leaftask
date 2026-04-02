namespace BuildingBlocks.Domain.Result;

public record Error
{
    public static readonly Error None = new(string.Empty, string.Empty, 200);
    public static readonly Error NullValue = new("General.Null", "Null value was provided", 500);


    public Error(string code, string description, int statusCode)
    {
        Code = code;
        Description = description;
        StatusCode = statusCode;
    }

    public string Code { get; }

    public string Description { get; }

    public int StatusCode { get; }
}
