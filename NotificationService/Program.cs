using CommonObjects.Models;
using Microsoft.EntityFrameworkCore;
using NotificationService;
using NotificationService.Data;
using NotificationService.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddScoped<INotificationProcessor, NotificationProcessor>();
builder.Services.AddScoped<INotification, NotificationProcessor>();
//builder.Services.AddSingleton<>
builder.Services.AddDbContext<NotificationProcessorDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("OrderDatabaseConnString"))
);
builder.Services.Configure<RabbitMQConfig>(builder.Configuration.GetSection(RabbitMQConfig.SectionName));

var rabbitHost = builder.Configuration["RabbitMQ:HostName"];
Console.WriteLine($"RabbitMQ Host: {rabbitHost}");
//_logger.LogInformation($"RabbitMQ Host: {rabbitHost}");
//builder.Services.AddDbContext<NotificationProcessorDbContext>(options =>
//    options.UseNpgsql(builder.Configuration.GetConnectionString("OrderDatabaseConnString")));
var host = builder.Build();
host.Run();
