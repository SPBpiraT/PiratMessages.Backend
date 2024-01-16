using FluentValidation;

namespace PiratMessages.Application.Messages.Commands.UpdateMessage
{
    public class UpdateMessageCommandValidator : AbstractValidator<UpdateMessageCommand>
    {
        public UpdateMessageCommandValidator()
        {
            RuleFor(updateMessageCommand =>
                updateMessageCommand.Id).NotEqual(Guid.Empty);
            RuleFor(updateMessageCommand =>
                updateMessageCommand.UserId).NotEqual(Guid.Empty);
        }
    }
}
