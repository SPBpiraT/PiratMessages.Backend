using RabbitMQ.Client;

namespace PiratMessages.Messaging.RabbitMQ
{
    internal class ChannelWrapper : IDisposable
    {
        private static volatile int counter = 0;

        private static readonly object staticLockObject = new();

        public ChannelWrapper(IModel channel)
        {
            Channel = channel;
            Id = Interlocked.Increment(ref counter);

            if (counter > 10000)
                lock (staticLockObject)
                    if (counter > 10000)
                        counter -= 10000;
        }

        public int Id { get; }

        public IModel? Channel { get; private set; }

        public void Dispose()
        {
            if (Channel == null)
                return;

            Channel.Abort();
            Channel.Dispose();
            Channel = null;
        }
    }
}