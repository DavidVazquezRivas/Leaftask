using FluentValidation;

namespace Modules.Chats.Application.Chats.Messages.Send;

public sealed class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageCommandValidator()
    {
        RuleFor(x => x.Content).NotEmpty().MaximumLength(20000);
    }
}
