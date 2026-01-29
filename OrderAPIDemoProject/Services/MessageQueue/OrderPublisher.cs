using CommonObjects.Models;
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
        public OrderPublisher(ILogger<OrderController> logger)
        {
            _factory = new ConnectionFactory() { HostName = "localhost" };
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

        public async void PublishOrder(Notification notification)
        {
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

            }
            catch (Exception ex)
            {
                //Log
                _logger.LogError("Order publishe error: " + ex.Message);
            }
        }
        public async ValueTask DisposeAsync()
        {
            _logger.LogDebug("Dispose");
            await _channel.DisposeAsync();
            await _connection.DisposeAsync();
        }
    }
}
