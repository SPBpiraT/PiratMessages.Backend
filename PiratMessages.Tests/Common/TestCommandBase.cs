using PiratMessages.Persistence;

namespace PiratMessages.Tests.Common
{
    public class TestCommandBase : IDisposable
    {
        protected readonly MessagesDbContext Context;

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
