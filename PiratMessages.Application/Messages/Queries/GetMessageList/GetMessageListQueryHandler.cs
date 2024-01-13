using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PiratMessages.Application.Interfaces;

namespace PiratMessages.Application.Messages.Queries.GetMessageList
{
    public class GetMessageListQueryHandler
        : IRequestHandler<GetMessageListQuery, MessageListVm>
    {
        private readonly IMessagesDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetMessageListQueryHandler(IMessagesDbContext dbContext, IMapper mapper) =>
            (_dbContext, _mapper) = (dbContext, mapper);

        public async Task<MessageListVm> Handle(GetMessageListQuery request,
            CancellationToken cancellationToken)
        {
            var messagesQuery = await _dbContext.Messages
                .Where(message => message.UserId == request.UserId)
                .ProjectTo<MessageLookupDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return new MessageListVm { Messages = messagesQuery };
        }
    }
}
