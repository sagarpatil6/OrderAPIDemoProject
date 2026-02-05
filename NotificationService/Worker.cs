//using CommonObjects.Models;
//using Microsoft.Extensions.Options;
//using NotificationService.Data;
//using NotificationService.Services;
//using RabbitMQ.Client;
//using RabbitMQ.Client.Events;
//using System.Text.Json;

//namespace NotificationService
//{
//    /// <summary>
//    /// Main class to Listen to the Notifications
//    /// </summary>
//    public class Worker : BackgroundService
//    {
//        private readonly ILogger<Worker> _logger = null;
//        private readonly IServiceScopeFactory _scopeFactory = null;
//        private IConnection _connection = null;
//        private IChannel _channel = null;
//        private readonly ConnectionFactory _factory = null;
//        private string QueueName = CommonObjects.Models.Constants.OrderQueueName;
//        private readonly RabbitMQConfig _rabbitMQConfig;

//        public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory, IOptions<RabbitMQConfig> options)
//        {
//            _logger = logger;
//            _scopeFactory = scopeFactory;
//            _rabbitMQConfig = options.Value;
//            _logger.LogInformation($"RabbitMQ Host: {_rabbitMQConfig.HostName}");
//            //Console.WriteLine($"RabbitMQ Host: {_rabbitMQConfig.HostName}");
//            _logger.LogInformation($"Delay");
//            Task.Delay(60000);
//            var factory = new ConnectionFactory() { HostName = _rabbitMQConfig.HostName };
//            _factory = factory;
//        }

//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {

//                try
//                {
//                    int retryCount = 0;
//                    while (retryCount < 3)
//                    {
//                        try
//                        {
//                            _logger.LogInformation("Init RabbitMQ Conection");
//                            _connection = await _factory.CreateConnectionAsync(stoppingToken);
//                            _channel = await _connection.CreateChannelAsync();
//                        }
//                        catch (Exception ex)
//                        {
//                            retryCount++;
//                            Console.WriteLine("RabbitMQ not ready, retrying in 10s...");
//                        _logger.LogInformation("Exception wait");
//                        //await Task.Delay(10000); // Wait 10 seconds
//                            await Task.Delay(20000);
//                        if (retryCount == 3)
//                            {
//                                throw;
//                            }
//                        }
//                    }
//                    _logger.LogInformation("Listen to Queue: " + QueueName);

//                    await _channel.QueueDeclareAsync(queue: QueueName,
//                                         durable: false, // Messages will not survive a server restart
//                                         exclusive: false,
//                                         autoDelete: false,
//                                         arguments: null);

//                    var orderConsumer = new AsyncEventingBasicConsumer(_channel);
//                    orderConsumer.ReceivedAsync += async (model, ea) =>
//                    {
//                        var body = ea.Body.ToArray();
//                        var message = System.Text.Encoding.UTF8.GetString(body);
//                        //Process the message
//                        _logger.LogInformation("Received message: {message}", message);
//                        try
//                        {
//                            var orderData = JsonSerializer.Deserialize<Notification>(message);
//                            //ToDo Save to DB
//                            if (orderData != null)
//                            {
//                                await HandleNotification(orderData);
//                            }
//                            else
//                            {
//                                _logger.LogWarning("No details found in the notification");
//                            }
//                            // Acknowledge the message
//                            await _channel.BasicAckAsync(ea.DeliveryTag, false);
//                            //await Task.Delay(40000);
//                        }
//                        catch (Exception ex)
//                        {
//                            //Log
//                            _logger.LogError("Error while recieving notifcation: " + ex.Message);
//                            _logger.LogDebug("Requeue Message");
//                            //Need to review this, added for demo project only
//                            await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
//                        }
//                        await Task.Yield();
//                    };
//                    await _channel.BasicConsumeAsync(
//                                                    queue: QueueName,
//                                                    autoAck: false, // Set to false because we are calling BasicAckAsync manually
//                                                    consumer: orderConsumer,
//                                                    cancellationToken: stoppingToken);
//                    //if (_logger.IsEnabled(LogLevel.Information))
//                    //{
//                    //    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
//                    //}


//                    //while (!stoppingToken.IsCancellationRequested)
//                    //{
//                    //    //if (_logger.IsEnabled(LogLevel.Information))
//                    //    //{
//                    //    //    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
//                    //    //}
//                    //    await Task.Delay(1000, stoppingToken);
//                    //}
//                }
//                catch (Exception ex)
//                {
//                    //Log
//                    _logger.LogError("Error in Message Queue Processing: " + ex.Message);
//                }

//            while (!stoppingToken.IsCancellationRequested)
//            {
//                await Task.Delay(1000, stoppingToken);
//            }
//        }

//        public async Task HandleNotification(Notification notification)
//        {
//            try
//            {
//                switch (notification.Type)
//                {
//                    case "ORDER_CREATED":
//                        await HandleOrderCreatedNotification(notification);
//                        break;
//                    case "ORDER_CANCELLED":
//                        await HandleOrderCancelledNotification(notification);
//                        break;
//                    default:
//                        //Log
//                        _logger.LogWarning("Received message with Type: {Type}", notification.Type);
//                        break;
//                }
//            }
//            catch (Exception ex)
//            {
//                //Log
//                _logger.LogError("Error while handling notification: " + ex.Message);
//            }
//        }

//        public async Task HandleOrderCreatedNotification(Notification notification)
//        {
//            try
//            {
//                INotificationProcessor _notificationProcessor;
//                INotification _notification;
//                // Create a new scope for each unit of work (e.g., each iteration)
//                using (var scope = _scopeFactory.CreateScope())
//                {
//                    var dbContext = scope.ServiceProvider.GetRequiredService<NotificationProcessorDbContext>();

//                    _notificationProcessor = new NotificationProcessor(dbContext, _logger);
//                    _notification = new NotificationProcessor(dbContext, _logger);
//                    // Use the dbContext instance within this scope
//                    await _notificationProcessor.UpdateOrderStatus(notification.Id, true);
//                    await _notification.SendEmail(notification.Email, notification);

//                    // The dbContext will be correctly disposed when the 'using' block ends
//                }
//            }
//            catch (Exception ex)
//            {
//                //Log
//                _logger.LogError("Error while handling notification: " + ex.Message);
//            }
//        }

//        public async Task HandleOrderCancelledNotification(Notification notification)
//        {
//            try
//            {
//                //ToDo
//            }
//            catch (Exception ex)
//            {
//                //Log
//                _logger.LogError("Error while handling notification: " + ex.Message);
//            }
//        }
//    }
//}

using CommonObjects.Models;
using Microsoft.Extensions.Options;
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
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private IConnection _connection;
        private IChannel _channel;
        private readonly ConnectionFactory _factory;
        private string QueueName = CommonObjects.Models.Constants.OrderQueueName;
        private readonly RabbitMQConfig _rabbitMQConfig;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory, IOptions<RabbitMQConfig> options)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _rabbitMQConfig = options.Value;

            _logger.LogInformation("RabbitMQ Host: {HostName}", _rabbitMQConfig.HostName);

            // Initialize the factory here, but don't attempt connection in the constructor
            _factory = new ConnectionFactory()
            {
                HostName = _rabbitMQConfig.HostName,
                //DispatchConsumersAsync = true // Required for AsyncEventingBasicConsumer in older versions, good practice in 8.0
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                bool connected = false;
                int retryCount = 0;

                // Loop until connected or service is stopped
                while (!connected && !stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        _logger.LogInformation("Attempting to connect to RabbitMQ (Attempt {Count})...", retryCount + 1);

                        _connection = await _factory.CreateConnectionAsync(stoppingToken);
                        _channel = await _connection.CreateChannelAsync();

                        connected = true;
                        _logger.LogInformation("Successfully connected to RabbitMQ.");
                    }
                    catch (Exception ex)
                    {
                        retryCount++;
                        _logger.LogWarning("RabbitMQ not ready. Retrying in 10s... Error: {Message}", ex.Message);

                        // Wait 10 seconds before next attempt
                        await Task.Delay(10000, stoppingToken);

                        if (retryCount >= 5) // Increased retries for Docker cold-starts
                        {
                            _logger.LogCritical("Could not connect to RabbitMQ after multiple attempts.");
                            throw;
                        }
                    }
                }

                _logger.LogInformation("Listen to Queue: {QueueName}", QueueName);

                await _channel.QueueDeclareAsync(queue: QueueName,
                                                 durable: false,
                                                 exclusive: false,
                                                 autoDelete: false,
                                                 arguments: null);

                var orderConsumer = new AsyncEventingBasicConsumer(_channel);
                orderConsumer.ReceivedAsync += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = System.Text.Encoding.UTF8.GetString(body);

                    _logger.LogInformation("Received message: {message}", message);

                    try
                    {
                        var orderData = JsonSerializer.Deserialize<Notification>(message);

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
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error while processing notification: {Message}", ex.Message);
                        _logger.LogDebug("Requeuing Message");

                        // Added for demo project only: Requeue on failure
                        await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
                    }
                    await Task.Yield();
                };

                await _channel.BasicConsumeAsync(queue: QueueName,
                                                 autoAck: false,
                                                 consumer: orderConsumer,
                                                 cancellationToken: stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fatal error in Message Queue Processing: {Message}", ex.Message);
            }

            // Keep the service alive while processing
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
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
                        _logger.LogWarning("Received message with Type: {Type}", notification.Type);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while handling notification: " + ex.Message);
            }
        }

        public async Task HandleOrderCreatedNotification(Notification notification)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<NotificationProcessorDbContext>();

                    var notificationProcessor = new NotificationProcessor(dbContext, _logger);
                    var emailNotifier = new NotificationProcessor(dbContext, _logger);

                    await notificationProcessor.UpdateOrderStatus(notification.Id, true);
                    await emailNotifier.SendEmail(notification.Email, notification);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while handling order created: " + ex.Message);
            }
        }

        public async Task HandleOrderCancelledNotification(Notification notification)
        {
            try
            {
                //ToDo
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while handling order cancelled: " + ex.Message);
            }
        }

        public override void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Notification Service is stopping.");

            if (_channel != null)
            {
                await _channel.CloseAsync();
            }

            if (_connection != null)
            {
                await _connection.CloseAsync();
            }

            await base.StopAsync(cancellationToken);
        }
    }
}
