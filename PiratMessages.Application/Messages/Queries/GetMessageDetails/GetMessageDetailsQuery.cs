using MediatR;

namespace PiratMessages.Application.Messages.Queries.GetMessageDetails
{
    public class GetMessageDetailsQuery : IRequest<MessageDetailsVm>
    {
        public Guid UserId { get; set; }
        public Guid Id { get; set; }
    }
}
