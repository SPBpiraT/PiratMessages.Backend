using PiratMessages.Application.Common.Messaging;

namespace PiratMessages.Application.Interfaces
{
    public interface IMessagingClient
    {
        void ExchangeDeclare(string name, ExchangeType type);

        Task ExchangeDeclareAsync(string name, ExchangeType type, CancellationToken cancellationToken = default);


        void QueueDeclare(string queue);

        Task QueueDeclareAsync(string queue, CancellationToken cancellationToken = default);


        void QueueBind(
            string queue,
            string exchange,
            string routingKey);

        Task QueueBindAsync(
            string queue,
            string exchange,
            string routingKey,
            CancellationToken cancellationToken = default);


        void QueueUnbind(
            string queue,
            string exchange,
            IEnumerable<string> routingKeys);

        Task QueueUnbindAsync(
            string queue,
            string exchange,
            IEnumerable<string> routingKeys,
            CancellationToken cancellationToken = default);


        void Publish<T>(string exchangeName, string routingKey, T obj);

        Task PublishAsync<T>(string exchangeName, string routingKey, T obj, CancellationToken cancellationToken = default);


        void PublishToQueue<T>(string queueName, T obj);

        Task PublishToQueueAsync<T>(string queueName, T obj, CancellationToken cancellationToken = default);


        Task ConsumeAsync<T>(
            string queue,
            bool autoAck,
            bool skipErrors,
            Func<T?, CancellationToken, Task> handler,
            int processingThreads = 1,
            CancellationToken cancellationToken = default);
    }
}
