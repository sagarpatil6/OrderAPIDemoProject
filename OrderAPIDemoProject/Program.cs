using CommonObjects.Models;
using Microsoft.EntityFrameworkCore;
using OrderAPIDemoProject.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext <OrderAPIDemoProject.Data.OrderDbContext> (options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("OrderDatabaseConnString")));
// Configure the RabbitMQ settings
builder.Services.Configure<RabbitMQConfig>(builder.Configuration.GetSection(RabbitMQConfig.SectionName));
builder.Services.AddScoped<IOrderService, OrderAPIDemoProject.Services.OrderService>();
builder.Services.AddScoped<IValidateOrderService, OrderAPIDemoProject.Services.OrderService>();
builder.Services.AddSingleton<IOrderPublisher, OrderAPIDemoProject.Services.MessageQueue.OrderPublisher>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
