using PiratMessages.Application.Common.Exceptions;
using PiratMessages.Application.Messages.Commands.UpdateMessage;
using PiratMessages.Tests.Common;
using Microsoft.EntityFrameworkCore;

namespace PiratMessages.Tests.Messages.Commands
{
    public class UpdateMessageCommandHandlerTests : TestCommandBase
    {
        [Fact]
        public async Task UpdateMessageCommandHandler_Success()
        {
            //Arrange
            var handler = new UpdateMessageCommandHandler(Context);

            //Act
            await handler.Handle(new UpdateMessageCommand
            {
                Id = MessagesContextFactory.MessageIdForUpdate,
                UserId = MessagesContextFactory.UserBId,
            }, CancellationToken.None);

            //Assert
            Assert.NotNull(await Context.Messages.SingleOrDefaultAsync(message =>
                message.Id == MessagesContextFactory.MessageIdForUpdate));
        }

        [Fact]
        public async Task UpdateMessageCommandHandler_FailOnWrongId()
        {
            //Arrange
            var handler = new UpdateMessageCommandHandler(Context);

            //Act

            //Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await handler.Handle(
                    new UpdateMessageCommand
                    {
                        Id = Guid.NewGuid(),
                        UserId = MessagesContextFactory.UserAId
                    },
                    CancellationToken.None));
        }

        [Fact]
        public async Task UpdateMessageCommandHandler_FailOnWrongUserId()
        {
            //Arrange
            var handler = new UpdateMessageCommandHandler(Context);

            //Act

            //Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await handler.Handle(
                    new UpdateMessageCommand
                    {
                        Id = MessagesContextFactory.MessageIdForUpdate,
                        UserId = MessagesContextFactory.UserAId
                    },
                    CancellationToken.None);
            });
        }
    }
}
