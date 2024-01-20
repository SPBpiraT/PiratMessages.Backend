using Microsoft.EntityFrameworkCore;
using PiratMessages.Tests.Common;
using PiratMessages.Application.Messages.Commands.CreateMessage;

namespace PiratMessages.Tests.Messages.Commands
{
    public class CreateMessageCommandHandlerTests : TestCommandBase
    {
        [Fact] 
        public async Task CreateMessageCommandHandler_Success()
        {
            //Arrange           
            var handler = new CreateMessageCommandHandler(Context);
            var messageDetails = "message details";
            var recipient = MessagesContextFactory.UserBId;

            //Act
            var messageId = await handler.Handle(
                new CreateMessageCommand
                {
                    UserId = MessagesContextFactory.UserAId,
                    DestinationUserId = recipient,
                    Details = messageDetails
                },
                CancellationToken.None);

            //Assert
            Assert.NotNull(
                await Context.Messages.SingleOrDefaultAsync(message =>
                message.Id == messageId && 
                message.DestinationUserId == MessagesContextFactory.UserBId &&
                message.Details == messageDetails));
        }
    }
}
