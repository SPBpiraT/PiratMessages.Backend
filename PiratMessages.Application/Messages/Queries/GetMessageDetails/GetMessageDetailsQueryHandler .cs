using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PiratMessages.Application.Common.Exceptions;
using PiratMessages.Application.Interfaces;
using PiratMessages.Domain;

namespace PiratMessages.Application.Messages.Queries.GetMessageDetails
{
    public class GetMessageDetailsQueryHandler
        : IRequestHandler<GetMessageDetailsQuery, MessageDetailsVm>
    {
        private readonly IMessagesDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetMessageDetailsQueryHandler(IMessagesDbContext dbContext,
            IMapper mapper) => (_dbContext, _mapper) = (dbContext, mapper);
        public async Task<MessageDetailsVm> Handle(GetMessageDetailsQuery request,
            CancellationToken cancellationToken)
        {
            var entity = await _dbContext.Messages
                .FirstOrDefaultAsync(message =>
                message.Id == request.Id, cancellationToken);

            if (entity == null || entity.UserId != request.UserId)
            {
                throw new NotFoundException(nameof(Message), request.Id);
            }

            return _mapper.Map<MessageDetailsVm>(entity);
        }
}
