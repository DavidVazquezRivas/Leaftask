namespace Modules.Chats.DrivingInfrastructure.Models.Requests;

public sealed record CreateChatRequest(Guid? OtherParticipantId, string OtherParticipantType);
