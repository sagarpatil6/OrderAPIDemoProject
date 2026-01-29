using CommonObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Data
{
    internal class NotificationProcessorDbContext : DbContext
    {
        public NotificationProcessorDbContext(DbContextOptions<NotificationProcessorDbContext> options) : base(options)
        {

        }
        public DbSet<Order> Orders { get; set; }
    }
}
