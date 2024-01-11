using MediatR;

namespace PiratMessages.Application.Messages.Commands.CreateMessage
{
    public class CreateMessageCommand : IRequest<Guid>
    {
        public Guid UserId { get; set; }
        public Guid DestinationUserId { get; set; }
        public string Details { get; set; }
    }
}
