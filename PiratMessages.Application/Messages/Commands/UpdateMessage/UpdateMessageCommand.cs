using MediatR;

namespace PiratMessages.Application.Messages.Commands.UpdateMessage
{
    public class UpdateMessageCommand : IRequest<Unit>
    {
        public Guid UserId { get; set; }
        public Guid Id { get; set; }
        public string Details { get; set; }
    }
}
