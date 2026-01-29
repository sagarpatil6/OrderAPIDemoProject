
using CommonObjects.Models;
using OrderAPIDemoProject.Models;

namespace OrderAPIDemoProject.Services.Interfaces
{
    public interface IOrderService
    {
        Task<List<Order>> GetAllOrdersAsync();
        Task<Order> GetOrderByIdAsync(int id);
        Task<Order> CreateAndPublishOrderAsync(Order order);
    }
}

