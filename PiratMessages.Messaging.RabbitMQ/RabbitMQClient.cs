using PiratMessages.Messaging.RabbitMQ.Options;
using PiratMessages.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Text.Json;
using ExchangeType = PiratMessages.Application.Common.Messaging.ExchangeType;

namespace PiratMessages.Messaging.RabbitMQ
{
    public class RabbitMQClient : IMessagingClient
    {
        private readonly ILogger<RabbitMQClient> _logger;
        private readonly RabbitMQOptions _options;
        private readonly ConnectionFactory _connectionFactory;
        private readonly AmqpTcpEndpoint[] _amqpTcpEndpoints;

        private readonly object _connectionChannelModificationLock = new();

        private bool _isAborted = false;
        private volatile IConnection? _connection;
        private volatile ChannelWrapper? _channel;

        public RabbitMQClient(
            ILogger<RabbitMQClient> logger,
            IOptions<RabbitMQOptions> options)
        {
            _logger = logger;
            _options = options.Value;

            var endpoints = GetEndpoints();
            _connectionFactory = CreateConnectionFactory(endpoints.First());

            _amqpTcpEndpoints = GetAmqpTcpEndpoints(endpoints, new SslOption());

        }

        public void ExchangeDeclare(string name, ExchangeType type)
        {
            var channelWrapper = GetOrCreateChannel();
            var channel = channelWrapper.Channel ?? throw new Exception($"{nameof(channelWrapper.Channel)} was null");

            lock (channel)
            {
                var threadId = Environment.CurrentManagedThreadId;
                var channelId = channelWrapper.Id;

                _logger.LogDebug(
                    "Declaring exchange {Name} thread {ThreadId} channel {ChannelId}",
                    name,
                    threadId,
                    channelId);

                channel.ExchangeDeclare(name, GetExchangeType(type));

                _logger.LogInformation(
                    "Declared exchange {Name} thread {ThreadId} channel {ChannelId}",
                    name,
                    threadId,
                    channelId);
            }
        }

        public Task ExchangeDeclareAsync(string name, ExchangeType type, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ExchangeDeclare(name, type);

            return Task.CompletedTask;
        }

        public void QueueDeclare(string queue)
        {
            var channel = GetOrCreateChannel().Channel;

            lock (channel)
            {
                var arguments = new Dictionary<string, object>();

                if (_options.MirrorQueues)
                    arguments.Add("ha-mode", "all");

                // if (expires > 0)
                //     arguments.Add("x-expires", expires);

                if (arguments.Count == 0)
                    arguments = null;

                var res = channel.QueueDeclare(
                    queue: queue,
                    durable: !string.IsNullOrWhiteSpace(queue),
                    exclusive: false,
                    autoDelete: string.IsNullOrWhiteSpace(queue),
                    arguments: arguments);

                _logger.LogDebug("Queue {Queue} declared", queue);
            }
        }

        public Task QueueDeclareAsync(string queue, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            QueueDeclare(queue);

            return Task.CompletedTask;
        }

        public void QueueBind(string queue, string exchange, string routingKey)
        {
            var channel = GetOrCreateChannel().Channel;
            lock (channel)
            {
                channel.QueueBind(queue, exchange, routingKey);
            }
        }

        public Task QueueBindAsync(string queue, string exchange, string routingKey, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            QueueBind(queue, exchange, routingKey);

            return Task.CompletedTask;
        }

        public void QueueUnbind(string queue, string exchange, IEnumerable<string> routingKeys)
        {
            var channel = GetOrCreateChannel().Channel;
            lock (channel)
            {
                foreach (var routingKey in routingKeys)
                {
                    _logger.LogInformation(
                        "QueueUnbind: {Queue}, {Exchange}, {RoutingKey}",
                        queue,
                        exchange,
                        routingKey);

                    channel.QueueUnbind(queue, exchange, routingKey);
                }
            }
        }

        public Task QueueUnbindAsync(
            string queue,
            string exchange,
            IEnumerable<string> routingKeys,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            QueueUnbind(queue, exchange, routingKeys);

            return Task.CompletedTask;
        }

        public void Publish<T>(string exchangeName, string routingKey, T obj)
        {
            var channel = GetOrCreateChannel().Channel;

            var raw = Serialize(obj);

            lock (channel)
            {
                channel.BasicPublish(exchangeName, routingKey, false, body: raw);
            }
        }

        public async Task PublishAsync<T>(string exchangeName, string routingKey, T obj, CancellationToken cancellationToken = default)
        {
            var channel = GetOrCreateChannel().Channel;

            var raw = await SerializeAsync(obj, cancellationToken);

            lock (channel)
            {
                channel.BasicPublish(exchangeName, routingKey, false, body: raw);
            }
        }

        public void PublishToQueue<T>(string queueName, T obj)
        {
            var channel = GetOrCreateChannel().Channel;

            var raw = Serialize(obj);

            lock (channel)
            {
                channel.BasicPublish(string.Empty, queueName, false, body: raw);
            }
        }

        public async Task PublishToQueueAsync<T>(string queueName, T obj, CancellationToken cancellationToken = default)
        {
            var channel = GetOrCreateChannel().Channel;

            var raw = await SerializeAsync(obj, cancellationToken);

            lock (channel)
            {
                channel.BasicPublish(string.Empty, queueName, false, body: raw);
            }
        }

        public async Task ConsumeAsync<T>(
            string queue,
            bool autoAck,
            bool skipErrors,
            Func<T?, CancellationToken, Task> handler,
            int processingThreads = 1,
            CancellationToken cancellationToken = default)
        {
            var consumerChannel = CreateChannel();
            var channel = consumerChannel.Channel ?? throw new Exception("Channel was null");

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.Received += async (ch, ea) =>
            {
                var acked = false;

                try
                {
                    if (autoAck)
                        channel.BasicAck(ea.DeliveryTag, false);

                    await Task.Yield();

                    var body = ea.Body.ToArray();

                    var obj = await DeserializeAsync<T>(body, cancellationToken);

                    await handler(obj, cancellationToken);

                    channel.BasicAck(ea.DeliveryTag, false);

                    acked = true;

                    await Task.Yield();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to consume");

                    if (!acked)
                        channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);

                    throw;
                }
            };

            var consumerTag = consumerChannel.Channel.BasicConsume(queue, autoAck, consumer);
        }

        #region private

        private ChannelWrapper CreateChannel()
        {
            var conn = GetOrCreateConnection(_options.MaxCreateChannelRetries);

            var originalChannel = conn.CreateModel();

            var channel = new ChannelWrapper(originalChannel);
            return channel;
        }

        private IConnection GetOrCreateConnection(int maxRetries)
        {
            if (_isAborted)
                throw new Exception("Rabbit client aborted");

            lock (_connectionChannelModificationLock)
            {
                if (_connection != null && _connection.IsOpen)
                    return _connection;

                SafeAbort();

                try
                {
                    _connection = CreateConnectionAuto(maxRetries);
                    SubscribeRecovery((IAutorecoveringConnection)_connection);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Handling Connection creation exception");
                    throw;
                }

                return _connection;
            }
        }

        private IConnection CreateConnectionAuto(int maxCreateRetries = 1)
        {
            IConnection connection = null;
            BrokerUnreachableException exception = null;
            for (var createAttempt = 1; createAttempt <= maxCreateRetries; createAttempt++)
            {
                try
                {
                    connection = _connectionFactory.CreateConnection(_amqpTcpEndpoints);
                    exception = null;
                    break;
                }
                catch (BrokerUnreachableException ex)
                {
                    _logger.LogError(
                        ex,
                        "Failed to create automatically on attempt {Attempt}: {ExcMessage}",
                        createAttempt,
                        ex.Message);

                    exception = ex;
                }
            }

            if (connection == null)
            {
                const string stage = "Auto create";
                Fail(stage, maxCreateRetries, exception);
                throw new Exception($"Failed on {stage} after {maxCreateRetries} attempts", exception);
            }

            return connection;
        }

        private ChannelWrapper GetOrCreateChannel() => GetOrCreateChannel(_options.MaxCreateChannelRetries);

        private ChannelWrapper GetOrCreateChannel(int maxRetries)
        {
            lock (_connectionChannelModificationLock)
            {
                // this forces channel to dispose, if connection is broken
                var conn = GetOrCreateConnection(maxRetries);

                if (_channel != null)
                    return _channel;

                var originalChannel = conn.CreateModel();

                _channel = new ChannelWrapper(originalChannel);
                var threadId = Thread.CurrentThread.ManagedThreadId;
                var channelId = _channel.Id;

                _logger.LogInformation("Channel created ID: {ChannelId} on thread {ThreadId}", channelId, threadId);

                return _channel;
            }
        }

        private ConnectionFactory CreateConnectionFactory(Uri endpoint)
        {
            var res = new ConnectionFactory
            {
                Uri = endpoint,
                RequestedHeartbeat = TimeSpan.FromSeconds(_options.DefaultRequestHeartbeatSeconds),
                AutomaticRecoveryEnabled = _options.AutomaticRecoveryEnabled,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(_options.NetworkRecoveryIntervalSeconds),
                AuthMechanisms = new IAuthMechanismFactory[] { new ExternalMechanismFactory(), new PlainMechanismFactory() },
                ConsumerDispatchConcurrency = _options.ConsumerDispatchConcurrency,
                DispatchConsumersAsync = true
            };

            if (!string.IsNullOrWhiteSpace(_options.Username))
            {
                res.UserName = _options.Username;
            }

            if (!string.IsNullOrWhiteSpace(_options.Password))
            {
                res.Password = _options.Password;
            }

            return res;
        }

        private void SafeAbort()
        {
            lock (_connectionChannelModificationLock)
            {
                var connection = _connection;
                var channel = _channel?.Channel;

                _channel = null;
                _connection = null;

                SafeComplexAbort(connection, channel);
            }
        }

        private void SafeAbort(IConnection connection)
        {
            lock (_connectionChannelModificationLock)
            {
                if (connection == _connection)
                    _connection = null;

                SafeComplexAbort(connection, null);
            }
        }

        private void SafeComplexAbort(IConnection? connection, IModel? channel)
        {
            if (channel != null)
            {
                try
                {
                    channel.Abort();
                    channel.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Safe Abort of channel failed");
                }
            }

            if (connection != null)
            {
                try
                {
                    connection.Abort();
                    connection.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Safe Abort of connection failed");
                }
            }
        }

        private void Fail(string stage, int recoverAttempt, Exception exc, IConnection? previousConnection = null)
        {
            _logger.LogError("Fail called on stage {Stage}", stage);

            if (_isAborted)
                return;

            var failException = new InvalidOperationException($"Failed on {stage} after {recoverAttempt} attempts", exc);

            _isAborted = true;

            // Abort the previous connection
            if (previousConnection != null)
                SafeAbort(previousConnection);

            // Abort the current connection
            SafeAbort();
        }

        private void SubscribeRecovery(IAutorecoveringConnection connection)
        {
            connection.ConnectionRecoveryError += (s, a) =>
            {
                _logger.LogCritical("Connection recovery was failed. Killing pod");

                Environment.ExitCode = 1;
                Environment.Exit(Environment.ExitCode);
            };

            connection.RecoverySucceeded += (s, a) =>
            {
                _logger.LogCritical("Connection recovery was successful. Anyway, killing host");

                Environment.ExitCode = 1;
                Environment.Exit(Environment.ExitCode);
            };
        }

        private List<Uri> GetEndpoints()
        {
            var result = new List<Uri>();

            var uri = $"amqp://{_options.Host}:{_options.Port}";

            var endpoint = new Uri(uri);

            result.Add(endpoint);

            return result;
        }

        #endregion

        #region static

        private static AmqpTcpEndpoint[] GetAmqpTcpEndpoints(IEnumerable<Uri> endpoints, SslOption sslOption)
        {
            return endpoints
                .Select(ep => new AmqpTcpEndpoint(ep, sslOption))
                .ToArray();
        }

        private static string GetExchangeType(ExchangeType type)
        {
            return type switch
            {
                ExchangeType.Direct => "direct",
                ExchangeType.Topic => "topic",
                _ => throw new Exception($"Unknown exchange type: {type}")
            };
        }

        private static byte[] Serialize<T>(T obj)
        {
            var memStream = new MemoryStream();
            JsonSerializer.Serialize(memStream, obj);

            return memStream.ToArray();
        }

        private static async Task<byte[]> SerializeAsync<T>(T obj, CancellationToken cancellationToken = default)
        {
            var memStream = new MemoryStream();
            await JsonSerializer.SerializeAsync(memStream, obj, cancellationToken: cancellationToken);

            return memStream.ToArray();
        }

        private static T? Deserialize<T>(byte[] bytes)
        {
            var obj = JsonSerializer.Deserialize<T>(new MemoryStream(bytes));

            return obj;
        }

        private static async Task<T?> DeserializeAsync<T>(byte[] bytes, CancellationToken cancellationToken = default)
        {
            var obj = await JsonSerializer.DeserializeAsync<T>(
                new MemoryStream(bytes),
                cancellationToken: cancellationToken);

            return obj;
        }

        #endregion
    }
}
