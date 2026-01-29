using CommonObjects.Models;
using Microsoft.EntityFrameworkCore;
using OrderAPIDemoProject.Controllers;
using OrderAPIDemoProject.Data;
using OrderAPIDemoProject.Models;
using OrderAPIDemoProject.Services.Interfaces;
using System;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace OrderAPIDemoProject.Services
{
    /// <summary>
    /// OrderService class to handle order related operations
    /// </summary>
    public class OrderService : IOrderService, IValidateOrderService
    {
        private readonly OrderDbContext _context = null;
        private readonly ILogger<OrderController> _logger = null;
        private const string pattern = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";
        private readonly IOrderPublisher _publisher = null;

        public OrderService(OrderDbContext context, ILogger<OrderController> logger, IOrderPublisher publisher)
        {
            _context = context;
            _logger = logger;
            _publisher = publisher;
        }

        /// <summary>
        /// Create and Publish Order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public async Task<Order> CreateAndPublishOrderAsync(Order order)
        {
            ////////Getting issues in PGSQL to save this DateTime with Kind Non Utc, so converting to Utc here
            //if (order.CreatedAt.Kind != DateTimeKind.Utc)
            //{
            //    // Convert to UTC from its current Kind (Local or Unspecified)
            //    order.CreatedAt = order.CreatedAt.ToUniversalTime();
            //}

            _logger.LogInformation("Save order in Database");
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            order.CreatedAtDateTIme = order.CreatedAt.ToLocalTime().DateTime;
            _logger.LogInformation("Publish Order");
            _publisher.PublishOrder(new Notification()
            {
                Id = order.Id,
                OrderId = order.Id.ToString(),
                Email = order.CustomerEmail,
                Type = Constants.NotificationTypeOrderCreated,
                Delivered = false,
                CreatedAt = order.CreatedAtDateTIme,
                ErrorMessage = ""
            });
            return order;
        }

        /// <summary>
        /// Get All the orders from DB
        /// </summary>
        /// <returns></returns>
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders.ToListAsync();
            orders.ForEach(order =>
            {
                order.CreatedAtDateTIme = order.CreatedAt.ToLocalTime().DateTime;
                //if (order.CreatedAt.Kind == DateTimeKind.Utc)
                //{
                //    order.CreatedAt = order.CreatedAt.ToLocalTime();
                //}
            }
            );
            return orders;
        }

        /// <summary>
        /// Get Order by Id from Db
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Order> GetOrderByIdAsync(int id)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == id);
            if (order != null /*&& order.CreatedAt.Kind == DateTimeKind.Utc*/)
            {
                order.CreatedAtDateTIme = order.CreatedAt.ToLocalTime().DateTime;
                //order.CreatedAt = order.CreatedAt.ToLocalTime();
            }
            return order;
        }

        /// <summary>
        /// Validation for Order
        /// </summary>
        /// <param name="newOrder"></param>
        /// <param name="validationErrors"></param>
        /// <returns></returns>
        public bool ValidateOrder(OrderDto newOrder, out string validationErrors)
        {
            validationErrors = "";
            if (newOrder == null || string.IsNullOrEmpty(newOrder.CustomerEmail) || string.IsNullOrEmpty(newOrder.ProductCode) || newOrder.Quantity <= 0)
            {
                if (Regex.IsMatch(newOrder.CustomerEmail, pattern, RegexOptions.IgnoreCase))
                {
                    return true;
                }
            }
            else
            {
                validationErrors = "Please provide valid input. required fields: customerEmail, productCode, quantity (greater than 0)";
            }
            return false;
        }
    }
}
