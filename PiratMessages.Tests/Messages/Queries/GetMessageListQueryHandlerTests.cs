using AutoMapper;
using PiratMessages.Tests.Common;
using PiratMessages.Persistence;
using PiratMessages.Application.Messages.Queries.GetMessageList;
using Shouldly;

namespace PiratMessages.Tests.Messages.Queries
{
    [Collection("QueryCollection")]
    public class GetMessageListQueryHandlerTests
    {
        private readonly MessagesDbContext Context;
        private readonly IMapper Mapper;

        public GetMessageListQueryHandlerTests(QueryTestFixture fixture)
        {
            Context = fixture.Context;
            Mapper = fixture.Mapper;
        }

        [Fact]
        public async Task GetMessageListQueryHandler_Success()
        {
            //Arrange
            var handler = new GetMessageListQueryHandler(Context, Mapper);

            //Act
            var result = await handler.Handle(
                new GetMessageListQuery
                {
                    UserId = MessagesContextFactory.UserBId
                },
                CancellationToken.None);

            //Assert

            result.ShouldBeOfType<MessageListVm>();
            result.Messages.Count.ShouldBe(2);
        }
    }
}
