using CommonObjects.Models;
using Microsoft.Extensions.Options;
using OrderAPIDemoProject.Controllers;
using OrderAPIDemoProject.Services.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace OrderAPIDemoProject.Services.MessageQueue
{
    /// <summary>
    /// Class to Publish the messages in the RabbitMQ Queue
    /// </summary>
    public class OrderPublisher : IOrderPublisher, IAsyncDisposable
    {
        private IConnection _connection = null;
        private IChannel _channel = null;
        private readonly ConnectionFactory _factory = null;
        private string QueueName = CommonObjects.Models.Constants.OrderQueueName;
        private readonly ILogger<OrderController> _logger = null;
        private readonly RabbitMQConfig _rabbitMQConfig;
        public OrderPublisher(ILogger<OrderController> logger, IOptions<RabbitMQConfig> options)
        {
            _rabbitMQConfig = options.Value;
            _factory = new ConnectionFactory() { HostName = _rabbitMQConfig.HostName };
            _logger = logger;
        }

        public async Task InitConnectionAsync()
        {
            //Check if connection and channel are already initialized
            if (_channel != null && _channel.IsOpen)
            {
                return;
            }
            _connection = await _factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            await _channel.QueueDeclareAsync(queue: QueueName,
                                 durable: false, // Messages will not survive a server restart
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            _logger.LogInformation("Order Queue Initialized");
        }

        public async Task<bool> PublishOrder(Notification notification)
        {
            bool isSucess = false;
            try
            {
                _logger.LogInformation("Publish order");
                await InitConnectionAsync();
                string message = JsonSerializer.Serialize(notification);
                var details = Encoding.UTF8.GetBytes(message);
                await _channel.BasicPublishAsync(exchange: "",
                                      routingKey: QueueName,
                                      body: details);
                //Log
                _logger.LogDebug("Order published");
                isSucess = true;
            }
            catch (Exception ex)
            {
                //Log
                _logger.LogError("Order publish error: " + ex.Message);
            }
            return isSucess;
        }
        public async ValueTask DisposeAsync()
        {
            _logger.LogDebug("Dispose");
            await _channel.DisposeAsync();
            await _connection.DisposeAsync();
        }
    }
}
