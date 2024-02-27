
namespace PiratMessages.Caching.Redis.Options
{
    public class RedisOptions
    {
        public static readonly string Section = "Caching:Redis";

        public string Host { get; set; } = "rediscache";

        public int Port { get; set; } = 6379;

        public int? Database { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public string Prefix { get; set; } = "PiratMessages";

        public int SyncTimeoutMs { get; set; } = 30000;

        public int RetryAttempts { get; set; } = 10;

        public int RetryTimeoutMs { get; set; } = 5000;
    }
}
