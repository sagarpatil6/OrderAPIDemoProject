using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonObjects.Models
{
    /// <summary>
    /// Constants used in the project
    /// </summary>
    public class Constants
    {
        public static readonly string OrderQueueName = "order-queue";
        public static readonly string NotificationTypeOrderCreated = "ORDER_CREATED";
        public static readonly string NotificationTypeOrderCancelled = "notification-queue";
        public static readonly string OrderStatusCreated = "CREATED";
        public static readonly string OrderStatusDelivered = "DELIVERED";
    }
}
