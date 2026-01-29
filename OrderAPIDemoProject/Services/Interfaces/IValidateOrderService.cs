
using CommonObjects.Models;
using OrderAPIDemoProject.Models;

namespace OrderAPIDemoProject.Services.Interfaces
{
    public interface IValidateOrderService
    {
        bool ValidateOrder(OrderDto order, out string validationErrors);
    }
}

