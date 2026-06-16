using FluentValidation;

namespace Modules.Chats.Application.Chats.Create;

public sealed class CreateChatCommandValidator : AbstractValidator<CreateChatCommand>
{
    public CreateChatCommandValidator()
    {
        RuleFor(x => x.OtherParticipantId).NotEmpty();
        RuleFor(x => x.OtherParticipantType)
            .Must(t => t is "person" or "agent")
            .WithMessage("OtherParticipantType must be 'person' or 'agent'.");
    }
}
