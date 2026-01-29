using CommonObjects.Models;
using Microsoft.EntityFrameworkCore;
using OrderAPIDemoProject.Models;

namespace OrderAPIDemoProject.Data
{
    public class OrderDbContext:DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        { 
        
        }

        public DbSet<Order> Orders { get; set; }
    }
}
