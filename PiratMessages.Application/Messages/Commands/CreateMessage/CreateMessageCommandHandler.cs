using MediatR;
using PiratMessages.Application.Common.Messaging;
using PiratMessages.Application.Interfaces;
using PiratMessages.Domain;

namespace PiratMessages.Application.Messages.Commands.CreateMessage
{
    public class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommand, Guid>
    {
        private readonly IMessagesDbContext _dbContext;
        private readonly IMessagingClient _messagingClient;
        private readonly ICachingClient _cachingClient;

        public CreateMessageCommandHandler(IMessagesDbContext dbContext, IMessagingClient messagingClient, ICachingClient cachingClient)
        {
            _dbContext = dbContext;
            _messagingClient = messagingClient;
            _cachingClient = cachingClient;
        }

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

            _messagingClient.ExchangeDeclare("test", ExchangeType.Topic);
            _messagingClient.QueueDeclareAsync("test");
            _messagingClient.PublishToQueue("test", "mymessage");

            await _cachingClient.SetAsync("test", $"{message.Id}", cancellationToken: cancellationToken);
            var test = await _cachingClient.GetAsync<string>("test", cancellationToken: cancellationToken);

            return new Guid(test);
            //return message.Id;
        }
    }
}
