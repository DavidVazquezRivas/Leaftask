namespace BuildingBlocks.Application.Abstractions;

public interface IUserContext
{
    Guid UserId { get; }
    bool IsAuthenticated { get; }
}
