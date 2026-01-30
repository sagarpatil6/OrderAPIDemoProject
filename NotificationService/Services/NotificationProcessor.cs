using CommonObjects.Models;
using Microsoft.EntityFrameworkCore;
using NotificationService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Services
{
    /// <summary>
    /// Notification Processor Class
    /// </summary>
    internal class NotificationProcessor : INotificationProcessor, INotification
    {
        private readonly NotificationProcessorDbContext _context = null;
        private readonly ILogger<Worker> _logger = null;

        public NotificationProcessor(NotificationProcessorDbContext context, ILogger<Worker> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task SendEmail(string to, Notification noticiation)
        {
            //Log
            _logger.LogInformation("Send Email");
            //ToDo
        }

        public async Task UpdateOrderStatus(int orderId, bool status)
        {
            try
            {
                _logger.LogInformation("Update Order status");
                //Get Order status from DB
                var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
                if(order != null)
                {
                    if(order.Status == Constants.OrderStatusCreated)
                    {
                        order.Status = Constants.OrderStatusDelivered;
                        await _context.SaveChangesAsync();
                    }
                    else if(order.Status == Constants.OrderStatusDelivered)
                    {
                        //Log already delivered, repeated event for this order
                        _logger.LogInformation("Order is already delivered, repeated notification");
                    }
                    else
                    {
                        //Log
                        _logger.LogWarning("Order Status recieved is different than Created/Cancelled: " + order.Status);
                    }
                }
                else
                {
                    //Log
                    _logger.LogWarning("Order not found: " + order.Id);
                }
            }
            catch (Exception ex)
            {
                //Log
                _logger.LogWarning("Error while updating order status: " + ex.Message);
            }
        }
    }
}
