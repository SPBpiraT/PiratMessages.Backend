﻿using PiratMessages.Tests.Common;
using PiratMessages.Application.Common.Exceptions;
using PiratMessages.Application.Messages.Commands.DeleteMessage;
using PiratMessages.Application.Messages.Commands.CreateMessage;

namespace PiratMessages.Tests.Messages.Commands
{
    public class DeleteMessageCommandHandlerTests : TestCommandBase
    {
        [Fact]
        public async Task DeleteMessageCommandHandler_Success()
        {
            //Arrange
            var handler = new DeleteMessageCommandHandler(Context);

            //Act
            await handler.Handle(new DeleteMessageCommand
            {
                Id = MessagesContextFactory.MessageIdForDelete,
                UserId = MessagesContextFactory.UserAId

            }, CancellationToken.None);

            //Assert
            Assert.Null(Context.Messages.SingleOrDefault(message =>
                message.Id == MessagesContextFactory.MessageIdForDelete));
        }

        [Fact]
        public async Task DeleteMessageCommandHandler_FailOnWrongId()
        {
            //Arrange
            var handler = new DeleteMessageCommandHandler(Context);

            //Act

            //Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await handler.Handle(
                    new DeleteMessageCommand
                    {
                        Id = Guid.NewGuid(),
                        UserId = MessagesContextFactory.UserAId
                    },
                    CancellationToken.None));
        }

        [Fact]
        public async Task DeleteMessageCommandHandler_FailOnWrongUserId()
        {
            //Arrange
            var deleteHandler = new DeleteMessageCommandHandler(Context);
            var createHandler = new CreateMessageCommandHandler(Context, _messagingClientMock.Object, _cachingClientMock.Object);

            var messageId = await createHandler.Handle(
                new CreateMessageCommand
                {
                    Details = "Details1",
                    UserId = MessagesContextFactory.UserAId
                }, CancellationToken.None);

            //Act

            //Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await deleteHandler.Handle(
                    new DeleteMessageCommand
                    {
                        Id = messageId,
                        UserId = MessagesContextFactory.UserBId
                    }, CancellationToken.None));
        }
    }
}
