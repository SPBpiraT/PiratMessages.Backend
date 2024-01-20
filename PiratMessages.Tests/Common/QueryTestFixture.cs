using AutoMapper;
using PiratMessages.Persistence;
using PiratMessages.Application.Interfaces;
using PiratMessages.Application.Common.Mappings;

namespace PiratMessages.Tests.Common
{
    public class QueryTestFixture
    {
        public MessagesDbContext Context;
        public IMapper Mapper;

        public QueryTestFixture()
        {
            Context = MessagesContextFactory.Create();
            var configurationProvider = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AssemblyMappingProfile(
                    typeof(IMessagesDbContext).Assembly));
            });

            Mapper = configurationProvider.CreateMapper();
        }

        public void Dispose()
        {
            MessagesContextFactory.Destroy(Context);
        }
    }

    [CollectionDefinition("QueryCollection")]
    public class QueryCollection : ICollectionFixture<QueryTestFixture> { }
}
