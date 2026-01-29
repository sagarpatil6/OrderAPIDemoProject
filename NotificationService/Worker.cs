using CommonObjects.Models;
using NotificationService.Data;
using NotificationService.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace NotificationService
{
    /// <summary>
    /// Main class to Listen to the Notifications
    /// </summary>
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger = null;
        private readonly IServiceScopeFactory _scopeFactory = null;
        private IConnection _connection = null;
        private IChannel _channel = null;
        private readonly ConnectionFactory _factory = null;
        private string QueueName = CommonObjects.Models.Constants.OrderQueueName;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _factory = factory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Init RabbitMQ Conection");
                _connection = await _factory.CreateConnectionAsync(stoppingToken);
                _channel = await _connection.CreateChannelAsync();
                _logger.LogInformation("Listent to Queue: " + QueueName);

                await _channel.QueueDeclareAsync(queue: QueueName,
                                     durable: false, // Messages will not survive a server restart
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var orderConsumer = new AsyncEventingBasicConsumer(_channel);
                orderConsumer.ReceivedAsync += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = System.Text.Encoding.UTF8.GetString(body);
                    //Process the message
                    _logger.LogInformation("Received message: {message}", message);
                    try
                    {
                        var orderData = JsonSerializer.Deserialize<Notification>(message);
                        //ToDo Save to DB
                        if (orderData != null)
                        {
                            await HandleNotification(orderData);
                        }
                        else
                        {
                            _logger.LogWarning("No details found in the notification");
                        }
                        // Acknowledge the message
                        await _channel.BasicAckAsync(ea.DeliveryTag, false);
                        //await Task.Delay(40000);
                    }
                    catch (Exception ex)
                    {
                        //Log
                        _logger.LogError("Error while recieving notifcation: " + ex.Message);
                        _logger.LogDebug("Requeue Message");
                        await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
                    }
                    await Task.Yield();
                };
                await _channel.BasicConsumeAsync(
                                                queue: QueueName,
                                                autoAck: false, // Set to false because we are calling BasicAckAsync manually
                                                consumer: orderConsumer,
                                                cancellationToken: stoppingToken);
                while (!stoppingToken.IsCancellationRequested)
                {
                    //if (_logger.IsEnabled(LogLevel.Information))
                    //{
                    //    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    //}
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                //Log
                _logger.LogError("Error in Message Queue Processing: " + ex.Message);
            }
        }

        public async Task HandleNotification(Notification notification)
        {
            try
            {
                switch (notification.Type)
                {
                    case "ORDER_CREATED":
                        await HandleOrderCreatedNotification(notification);
                        break;
                    case "ORDER_CANCELLED":
                        await HandleOrderCancelledNotification(notification);
                        break;
                    default:
                        //Log
                        _logger.LogWarning("Received message with Type: {Type}", notification.Type);
                        break;
                }
            }
            catch (Exception ex)
            {
                //Log
                _logger.LogError("Error while handling notification: " + ex.Message);
            }
        }

        public async Task HandleOrderCreatedNotification(Notification notification)
        {
            try
            {
                INotificationProcessor _notificationProcessor;
                INotification _notification;
                // Create a new scope for each unit of work (e.g., each iteration)
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<NotificationProcessorDbContext>();

                    _notificationProcessor = new NotificationProcessor(dbContext, _logger);
                    _notification = new NotificationProcessor(dbContext, _logger);
                    // Use the dbContext instance within this scope
                    await _notificationProcessor.UpdateOrderStatus(notification.Id, true);
                    await _notification.SendEmail(notification.Email, notification);

                    // The dbContext will be correctly disposed when the 'using' block ends
                }
            }
            catch (Exception ex)
            {
                //Log
                _logger.LogError("Error while handling notification: " + ex.Message);
            }
        }

        public async Task HandleOrderCancelledNotification(Notification notification)
        {
            try
            {
                //ToDo
            }
            catch (Exception ex)
            {
                //Log
                _logger.LogError("Error while handling notification: " + ex.Message);
            }
        }
    }
}
