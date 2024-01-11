using MediatR;
using Microsoft.EntityFrameworkCore;
using PiratMessages.Application.Common.Exceptions;
using PiratMessages.Application.Interfaces;
using PiratMessages.Domain;

namespace PiratMessages.Application.Messages.Commands.UpdateMessage
{
    public class UpdateMessageCommandHandler
        : IRequestHandler<UpdateMessageCommand, Unit>
    {
        private readonly IMessagesDbContext _dbContext;
        public UpdateMessageCommandHandler(IMessagesDbContext dbContext) =>
            _dbContext = dbContext;

        public async Task<Unit> Handle(UpdateMessageCommand request,
            CancellationToken cancellationToken)
        {
            var entity =
                await _dbContext.Messages.FirstOrDefaultAsync(message =>
                message.Id == request.Id, cancellationToken);

            if (entity == null || entity.UserId != request.UserId)
            {
                throw new NotFoundException(nameof(Message), request.Id);
            }

            entity.Details = request.Details;
            entity.EditDate = DateTime.Now;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
