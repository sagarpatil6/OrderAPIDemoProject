using CommonObjects.Models;
using Microsoft.AspNetCore.Mvc;
using OrderAPIDemoProject.Models;
using OrderAPIDemoProject.Services.Interfaces;
using System.Text.Json;

namespace OrderAPIDemoProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger = null;
        private readonly IOrderService _orderService = null;
        private readonly IValidateOrderService _validationService = null;

        public OrderController(ILogger<OrderController> logger, IOrderService orderService, IValidateOrderService validationService)
        {
            _logger = logger;
            _orderService = orderService;
            _validationService = validationService;
        }

        [HttpGet(Name = "GetOrder")]
        //[Route("/api/orders")]
        public async Task<ActionResult<List<Order>>> Get()
        {
            try
            {
                _logger.LogInformation("Get all order details");
                return Ok(await _orderService.GetAllOrdersAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while getting order details: {ErrorMessage}", ex.Message);
                // Return a 500 Internal Server Error with a generic message for the client
                return StatusCode(500, new { message = "An internal server error occurred", detail = ex.Message });
            }
        }


        [HttpGet("{id}")]
        //[Route("/api/orders/{id}")]
        public async Task<ActionResult<Order>> GetOrderById(int id)
        {
            try
            {
                _logger.LogInformation("Get order details for Id: " + id);
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    return NotFound();
                }
                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while getting order details: {ErrorMessage}", ex.Message);
                // Return a 500 Internal Server Error with a generic message for the client
                return StatusCode(500, new { message = "An internal server error occurred", detail = ex.Message });
            }
        }


        [HttpPost(Name = "CreateOrder")]
        //[Route("/api/orders")]
        public async Task<ActionResult<Order>> Create([FromBody] OrderDto newOrder)
        {
            try
            {
                _logger.LogInformation("Create new order for product {ProductCode} for customer {CustomerEmail}", newOrder.ProductCode, newOrder.CustomerEmail);
                _logger.LogDebug(JsonSerializer.Serialize(newOrder));
                if (_validationService.ValidateOrder(newOrder, out string message))
                {
                    _logger.LogWarning(message);
                    return BadRequest(message);
                }

                //Set Values for required fields
                var allOrders = await _orderService.GetAllOrdersAsync();
                var newOrderObj = new Order()
                {
                    CustomerEmail = newOrder.CustomerEmail,
                    ProductCode = newOrder.ProductCode,
                    Quantity = newOrder.Quantity
                };
                //ToDo Id to be auto generated from DB
                //newOrderObj.Id = allOrders.Count > 0 ? allOrders.Max(o => o.Id) + 1 : 1;
                newOrderObj.Status = Constants.OrderStatusCreated;
                newOrderObj.CreatedAt = DateTimeOffset.UtcNow;


                var orderObj = await _orderService.CreateAndPublishOrderAsync(newOrderObj);
                if(orderObj != null)
                {
                    _logger.LogInformation("Order Created Successfully");
                    return CreatedAtAction(nameof(GetOrderById), new { id = orderObj.Id }, newOrderObj);
                }
                else
                {
                    return StatusCode(500, new { message = "An internal server error occurred:", detail = "Failure while creating order in DB" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while creating order: {ErrorMessage}", ex.Message);
                // Return a 500 Internal Server Error with a generic message for the client
                return StatusCode(500, new { message = "An internal server error occurred", detail = ex.Message });
            }
        }
    }
}
