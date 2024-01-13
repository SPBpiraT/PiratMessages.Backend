using MediatR;

namespace PiratMessages.Application.Messages.Commands.DeleteMessage
{
    public class DeleteMessageCommand : IRequest<Unit>
    {
        public Guid UserId { get; set; }
        public Guid Id { get; set; }
    }
}
