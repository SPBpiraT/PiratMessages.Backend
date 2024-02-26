
namespace PiratMessages.Application.Interfaces
{
    public interface ICachingClient
    {
        Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

        T? Get<T>(string key) where T : class;

        string? GetRaw(string key);

        Task<string?> GetRawAsync(string key, CancellationToken cancellationToken = default);

        Task<bool> SetAsync<T>(
            string key,
            T value,
            TimeSpan? expiry = null,
            bool overwriteIfExists = true,
            CancellationToken cancellationToken = default) where T : class;

        bool Set<T>(string key, T value, TimeSpan? expiry = null, bool overwriteIfExists = true) where T : class;

        bool SetRaw(string key, string value, TimeSpan? expiry = null, bool overwriteIfExists = true);

        Task DeleteAsync(string key, CancellationToken cancellationToken = default);

        void Delete(string key);
    }
}
