using PiratMessages.Persistence;
using Moq;
using PiratMessages.Application.Interfaces;

namespace PiratMessages.Tests.Common
{
    public class TestCommandBase : IDisposable
    {
        protected readonly MessagesDbContext Context;
        protected readonly Mock<IMessagingClient> _messagingClientMock = new Mock<IMessagingClient>();
        protected readonly Mock<ICachingClient> _cachingClientMock = new Mock<ICachingClient>();

        public TestCommandBase()
        {
            Context = MessagesContextFactory.Create();
        }

        public void Dispose()
        {
            MessagesContextFactory.Destroy(Context);
        }
    }
}
