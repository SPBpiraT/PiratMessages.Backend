using FluentValidation;

namespace PiratMessages.Application.Messages.Commands.CreateMessage
{
    public class CreateMessageCommandValidator : AbstractValidator<CreateMessageCommand>
    {
        public CreateMessageCommandValidator()
        {
            RuleFor(createMessageCommand =>
                createMessageCommand.UserId).NotEqual(Guid.Empty);
            RuleFor(createMessageCommand =>
                createMessageCommand.DestinationUserId).NotEqual(Guid.Empty);
        }
    }
}
