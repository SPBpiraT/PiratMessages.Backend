using MediatR;

namespace PiratMessages.Application.Messages.Queries.GetMessageList
{
    public class GetMessageListQuery : IRequest<MessageListVm>
    {
        public Guid UserId { get; set; }
    }
}
