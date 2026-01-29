using CommonObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Services
{
    public interface INotificationProcessor
    {
        Task UpdateOrderStatus(int orderId, bool status);
    }

    public interface INotification
    {
        Task SendEmail(string to, Notification noticiation);
    }
}
