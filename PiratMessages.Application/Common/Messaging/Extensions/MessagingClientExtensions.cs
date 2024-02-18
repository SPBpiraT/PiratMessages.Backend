using PiratMessages.Application.Interfaces;

namespace PiratMessages.Application.Common.Messaging.Extensions
{
    public static class MessagingClientExtensions
    {
        public static void QueueDeclare<T>(this IMessagingClient messagingClient)
            => messagingClient.QueueDeclare(typeof(T).Name);

        public static Task QueueDeclareAsync<T>(this IMessagingClient messagingClient, CancellationToken cancellationToken = default)
            => messagingClient.QueueDeclareAsync(typeof(T).Name, cancellationToken);

        public static void PublishToQueue<T>(this IMessagingClient messagingClient, T obj)
            => messagingClient.PublishToQueue(typeof(T).Name, obj);

        public static Task PublishToQueueAsync<T>(this IMessagingClient messagingClient, T obj, CancellationToken cancellationToken = default)
            => messagingClient.PublishToQueueAsync(typeof(T).Name, obj, cancellationToken);

        public static Task ConsumeAsync<T>(
            this IMessagingClient messagingClient,
            bool autoAck,
            bool skipErrors,
            Func<T?, CancellationToken, Task> handler,
            int processingThreads = 1,
            CancellationToken cancellationToken = default)
            => messagingClient.ConsumeAsync<T>(
                typeof(T).Name,
                autoAck,
                skipErrors,
                handler,
                processingThreads,
                cancellationToken);
    }
}
