using BuildingBlocks.Domain.Result;

namespace Modules.Chats.Domain.Errors;

public static class ChatErrors
{
    public static readonly Error ChatNotFound =
        new("Chat.NotFound", "The specified chat was not found.", 404);

    public static readonly Error NotParticipant =
        new("Chat.NotParticipant", "You are not a participant in this chat.", 403);

    public static readonly Error AlreadyExists =
        new("Chat.AlreadyExists", "A chat with this participant already exists.", 409);

    public static readonly Error ParticipantNotFound =
        new("Chat.ParticipantNotFound", "The specified participant was not found.", 404);
}
