using Microsoft.Extensions.Options;
using PiratMessages.Application.Interfaces;
using PiratMessages.Caching.Redis.Options;
using StackExchange.Redis;
using System.Net;
using System.Text.Json;
using Serilog;

namespace PiratMessages.Caching.Redis.Client
{
    public sealed class RedisClient : ICachingClient
    {
        private readonly RedisOptions _redisOptions;
        private readonly ConfigurationOptions _config;

        private readonly object _lockRoot = new();
        private IDatabase? _db;
        private bool _initialized;
        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

        public RedisClient(IOptions<RedisOptions> redisOptions)
        {
            _redisOptions = redisOptions.Value;

            _config = new ConfigurationOptions
            {
                EndPoints = new EndPointCollection(new List<EndPoint>
                {
                    new DnsEndPoint(_redisOptions.Host, _redisOptions.Port)
                }),
                SyncTimeout = _redisOptions.SyncTimeoutMs,
                Password = _redisOptions.Password,
                User = _redisOptions.User,
                ConnectRetry = 3,
                DefaultDatabase = _redisOptions.Database
            };
        }


        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            return await DoWithTimeoutAsync(nameof(GetAsync), key, async (ct) =>
            {
                _db = await EnsureConnectionAsync();

                var finalKey = GetFinalKey(key);

                ct.ThrowIfCancellationRequested();

                var raw = await _db.StringGetAsync(finalKey);
                return Deserialize<T>(raw);
            }, cancellationToken);
        }

        public T? Get<T>(string key) where T : class
        {
            var raw = GetRaw(key);
            return Deserialize<T>(raw);
        }

        public string? GetRaw(string key)
        {
            return DoWithTimeout(nameof(GetRaw), key, () =>
            {
                _db = EnsureConnection();

                var finalKey = GetFinalKey(key);
                var raw = _db.StringGet(finalKey);

                return raw;
            });
        }

        public async Task<string?> GetRawAsync(string key, CancellationToken cancellationToken = default)
        {
            return await DoWithTimeoutAsync(nameof(GetRaw), key, async (ct) =>
            {
                _db = await EnsureConnectionAsync();

                var finalKey = GetFinalKey(key);

                ct.ThrowIfCancellationRequested();

                var raw = await _db.StringGetAsync(finalKey);

                return raw;
            }, cancellationToken);
        }

        public async Task<bool> SetAsync<T>(
            string key,
            T value,
            TimeSpan? expiry = null,
            bool overwriteIfExists = true,
            CancellationToken cancellationToken = default) where T : class
        {
            return await DoWithTimeoutAsync(nameof(SetAsync), key, async (ct) =>
            {
                _db = await EnsureConnectionAsync();

                var finalKey = GetFinalKey(key);

                var raw = Serialize(value);

                ct.ThrowIfCancellationRequested();

                return await _db.StringSetAsync(finalKey, raw, expiry, overwriteIfExists ? When.Always : When.NotExists);
            }, cancellationToken);
        }

        public bool Set<T>(string key, T value, TimeSpan? expiry = null, bool overwriteIfExists = true) where T : class
        {
            var raw = Serialize(value);
            return SetRaw(key, raw, expiry, overwriteIfExists);
        }

        public bool SetRaw(string key, string value, TimeSpan? expiry = null, bool overwriteIfExists = true)
        {
            return DoWithTimeout(nameof(SetRaw), key, () =>
            {
                _db = EnsureConnection();

                var finalKey = GetFinalKey(key);

                return _db.StringSet(finalKey, value, expiry, overwriteIfExists ? When.Always : When.NotExists);
            });
        }

        public async Task DeleteAsync(string key, CancellationToken cancellationToken = default)
        {
            await DoWithTimeoutAsync(nameof(DeleteAsync), key, async (ct) =>
            {
                _db = await EnsureConnectionAsync();

                var finalKey = GetFinalKey(key);

                ct.ThrowIfCancellationRequested();

                return await _db.KeyDeleteAsync(finalKey);
            }, cancellationToken);
        }

        public void Delete(string key)
        {
            DoWithTimeout(nameof(Delete), key, () =>
            {
                _db = EnsureConnection();

                var finalKey = GetFinalKey(key);

                return _db.KeyDelete(finalKey);
            });
        }

        private async Task<T> DoWithTimeoutAsync<T>(
            string operation,
            string key,
            Func<CancellationToken, Task<T>> action,
            CancellationToken cancellationToken = default)
        {
            var attemptNum = 0;

            while (attemptNum < _redisOptions.RetryAttempts)
            {
                try
                {
                    return await action(cancellationToken);
                }
                catch (TimeoutException ex)
                {
                    await Task.Delay(_redisOptions.RetryTimeoutMs, cancellationToken);

                    Log.Warning(
                        ex,
                        "Timeout while trying to {Operation} by key {Key}, attempt {AttemptNum}",
                        operation, key, attemptNum);

                    attemptNum++;
                }
            }

            throw new TimeoutException($"Failed to {operation} by key {key} after {_redisOptions.RetryAttempts} retries");
        }

        private T DoWithTimeout<T>(string operation, string key, Func<T> action)
        {
            var attemptNum = 0;

            while (attemptNum < _redisOptions.RetryAttempts)
            {
                try
                {
                    return action();
                }
                catch (TimeoutException ex)
                {
                    Thread.Sleep(_redisOptions.RetryTimeoutMs);

                    Log.Warning(
                        ex,
                        "Timeout while trying to {Operation} by key {Key}, attempt {AttemptNum}",
                        operation, key, attemptNum);

                    attemptNum++;
                }
            }

            throw new TimeoutException($"Failed to {operation} by key {key} after {_redisOptions.RetryAttempts} retries");
        }

        private async Task<IDatabase> EnsureConnectionAsync()
        {
            if (_initialized)
                return _db;

            await _semaphoreSlim.WaitAsync();
            try
            {
                if (_initialized)
                    return _db;

                var redis = await ConnectionMultiplexer.ConnectAsync(_config);
                _db = redis.GetDatabase();
                _initialized = true;
                return _db;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private IDatabase EnsureConnection()
        {
            if (_initialized)
                return _db;

            lock (_lockRoot)
            {
                if (_initialized)
                    return _db;

                var redis = ConnectionMultiplexer.Connect(_config);
                _db = redis.GetDatabase();

                _initialized = true;
            }

            return _db;
        }

        private static RedisValue Serialize<T>(T value)
        {
            if (value == null)
                return RedisValue.Null;

            return (RedisValue)JsonSerializer.Serialize(value);
        }

        private static T? Deserialize<T>(RedisValue raw) => raw.IsNull
            ? default(T)
            : JsonSerializer.Deserialize<T>(raw);

        private string GetFinalKey(string key) => $"{_redisOptions.Prefix}.{key}";
    }
}
