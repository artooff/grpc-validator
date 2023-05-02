using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ
{
    public class RabbitMQRPCReciever<TGet, TSend>
        where TGet : IRabbitMQMessage, new()
        where TSend : IRabbitMQMessage
    {
        public ILogger Logger { get; set; }
        public IConnection Connection { get; set; }
        public IModel Channel { get; set; }
        public string RequestQueueName { get; set; }
        public AsyncEventingBasicConsumer Consumer { get; set; }

        private ConcurrentDictionary<string, TaskCompletionSource<string>> _requestTasks = new();

        public Func<TGet, Task<TSend>> RPC { get; set; }

        public RabbitMQRPCReciever(string requestQueueName, ILogger logger)
        {
          //  RequestQueueName = configuration?.GetSection("RequestQueueValue").Value;
            RequestQueueName = requestQueueName;
            Logger = logger;

            var factory = new ConnectionFactory()
            {
                HostName =  "localhost",
                DispatchConsumersAsync = true
            };

            Connection = factory.CreateConnection();
            Channel = Connection.CreateModel();

            Channel.QueueDeclare(RequestQueueName, exclusive:false, autoDelete: true);

            Consumer = new AsyncEventingBasicConsumer(Channel);
            Consumer.Received += OnReceived;

            Channel.BasicConsume(RequestQueueName, false, Consumer);
        }

        private async Task OnReceived(object? sender, BasicDeliverEventArgs e)
        {
            TGet message = new();
            var messageBytesArray = e.Body.ToArray();
            message.GetFromByteArray(messageBytesArray);

            var requestProperties = e.BasicProperties;
            var replyProperties = Channel.CreateBasicProperties();
            replyProperties.CorrelationId = requestProperties.CorrelationId;

            var response = await RpcCallAsync(message);

            Channel.BasicPublish("", requestProperties.ReplyTo, replyProperties, response.ToByteArray());
        }

        private async Task<TSend> RpcCallAsync(TGet message, CancellationToken cancellationToken = default)
        {
            return await RPC.Invoke(message);
        }
    }
}
