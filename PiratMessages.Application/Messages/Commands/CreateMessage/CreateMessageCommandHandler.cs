using MediatR;
using PiratMessages.Application.Interfaces;
using PiratMessages.Domain;

namespace PiratMessages.Application.Messages.Commands.CreateMessage
{
    public class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommand, Guid>
    {
        private readonly IMessagesDbContext _dbContext;

        public CreateMessageCommandHandler(IMessagesDbContext dbContext) =>
            _dbContext = dbContext;

        public async Task<Guid> Handle(CreateMessageCommand request,
            CancellationToken cancellationToken)
        {
            var message = new Message
            {
                UserId = request.UserId,
                DestinationUserId = request.DestinationUserId,
                Details = request.Details,
                Id = Guid.NewGuid(),
                CreationDate = DateTime.Now,
                EditDate = null
            };

            await _dbContext.Messages.AddAsync(message, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return message.Id;
        }
    }
}
