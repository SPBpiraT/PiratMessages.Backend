using PiratMessages.Application.Interfaces;

namespace PiratMessages.WebApi.HostedServices
{
    public class RedisTestHostedService : IHostedService
    {
        private readonly ICachingClient _cachingClient;

        public RedisTestHostedService(ICachingClient cachingClient)
        {
            _cachingClient = cachingClient;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _cachingClient.SetAsync("test", "value", cancellationToken: cancellationToken);
            var test = await _cachingClient.GetAsync<string>("test", cancellationToken: cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
