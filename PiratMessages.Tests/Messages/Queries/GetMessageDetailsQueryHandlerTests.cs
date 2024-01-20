using AutoMapper;
using PiratMessages.Tests.Common;
using PiratMessages.Persistence;
using PiratMessages.Application.Messages.Queries.GetMessageDetails;
using Shouldly;

namespace PiratMessages.Tests.Messages.Queries
{
    [Collection("QueryCollection")]
    public class GetMessageDetailsQueryHandlerTests
    {
        private readonly MessagesDbContext Context;
        private readonly IMapper Mapper;

        public GetMessageDetailsQueryHandlerTests(QueryTestFixture fixture)
        {
            Context = fixture.Context;
            Mapper = fixture.Mapper;
        }

        [Fact]
        public async Task GetMessageDetailsQueryHandler_Success()
        {
            //Arrange
            var handler = new GetMessageDetailsQueryHandler(Context, Mapper);

            //Act
            var result = await handler.Handle(
                new GetMessageDetailsQuery
                {
                    UserId = MessagesContextFactory.UserBId,
                    Id = Guid.Parse("6A651058-11F4-4A38-9A79-ACBB497B092C")
                },
                CancellationToken.None);

            //Assert
            result.ShouldBeOfType<MessageDetailsVm>();
            result.CreationDate.ShouldBe(DateTime.Today);
        }
    }
}
