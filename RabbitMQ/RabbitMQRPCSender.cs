using Google.Protobuf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;

namespace RabbitMQ
{
    public class RabbitMQRPCSender<TSend, TGet>
        where TGet : IRabbitMQMessage, new()
        where TSend : IRabbitMQMessage
    {
        public ILogger Logger { get; set; }
        public IConnection Connection { get; set; }
        public IModel Channel { get; set; }
        public string RequestQueueName { get; set; }
        public string ReplyQueueName { get; set; }
        public EventingBasicConsumer Consumer { get; set; }

        private ConcurrentDictionary<string, TaskCompletionSource<TGet>> _requestTasks = new();
        public RabbitMQRPCSender(IConfigurationSection configuration, ILogger logger)
        {
            Logger = logger;

            RequestQueueName = configuration.GetSection("RequestQueueName").Value;
            var factory = new ConnectionFactory() { HostName = configuration.GetSection("Hostname").Value};
            Connection = factory.CreateConnection();
            Channel = Connection.CreateModel();

            var replyQueue = Channel.QueueDeclare("", exclusive: true, autoDelete:true);
            ReplyQueueName = replyQueue.QueueName;

            Channel.QueueDeclare(RequestQueueName, exclusive: false);

            Consumer = new EventingBasicConsumer(Channel);
            Consumer.Received += OnReceived;
        }

        public Task<TGet> CallAsync(TSend message, CancellationToken cancellationToken = default)
        {
           // Logger?.LogInformation("requesting" + message);

            var messageCorrelationId = Guid.NewGuid().ToString();
            var task = new TaskCompletionSource<TGet>();
            _requestTasks.TryAdd(messageCorrelationId, task);

            var properties = Channel.CreateBasicProperties();
            properties.CorrelationId = messageCorrelationId;
            properties.ReplyTo = ReplyQueueName;

            var messageBytes = message.ToByteArray();
            Channel.BasicPublish("", RequestQueueName, properties, messageBytes);
            Channel.BasicConsume(Consumer, ReplyQueueName, true);

            return task.Task;
        }
        
        private void OnReceived(object? sender, BasicDeliverEventArgs ea)
        {
            if(_requestTasks.TryRemove(ea.BasicProperties.CorrelationId, out var task))
            {
                task?.TrySetResult(OnReceivedSuccess(sender, ea));
            }
            return;
        }

        private TGet OnReceivedSuccess(object? sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var response = new TGet();
            response.GetFromByteArray(body);
            return response;
        }
    }
}