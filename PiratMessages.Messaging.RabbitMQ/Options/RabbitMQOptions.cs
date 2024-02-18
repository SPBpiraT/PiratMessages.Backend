using RabbitMQ.Client;

namespace PiratMessages.Messaging.RabbitMQ.Options
{
    public class RabbitMQOptions
    {
        public static readonly string Section = "Messaging:RabbitMQ";

        public string Host { get; set; } = "rabbitmq";

        public int Port { get; set; } = 5672;

        public string? Username { get; set; }

        public string? Password { get; set; }

        public int MaxCreateChannelRetries { get; set; } = 3;

        public int DefaultRequestHeartbeatSeconds { get; set; } = 60;

        public bool AutomaticRecoveryEnabled { get; set; } = true;

        public int NetworkRecoveryIntervalSeconds { get; set; } = 10;

        public int ConsumerDispatchConcurrency { get; set; } = 1;

        public bool MirrorQueues { get; set; } = false;
    }
}