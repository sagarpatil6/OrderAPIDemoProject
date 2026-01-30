using CommonObjects.Models;
using OrderAPIDemoProject.Models;

namespace OrderAPIDemoProject.Services.Interfaces
{
    public interface IOrderPublisher
    {
        Task<bool> PublishOrder(Notification notification);
    }
}