using MediatR;
using PiratMessages.Application.Common.Exceptions;
using PiratMessages.Application.Interfaces;
using PiratMessages.Domain;

namespace PiratMessages.Application.Messages.Commands.DeleteMessage
{
    public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand, Unit>
    {
        private readonly IMessagesDbContext _dbContext;
        public DeleteMessageCommandHandler(IMessagesDbContext dbContext) =>
            _dbContext = dbContext;

        public async Task<Unit> Handle(DeleteMessageCommand request,
            CancellationToken cancellationToken)
        {
            var entity = await _dbContext.Messages.FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity == null || entity.UserId != request.UserId)
            {
                throw new NotFoundException(nameof(Message), request.Id);
            }

            _dbContext.Messages.Remove(entity);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
