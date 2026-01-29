using System.ComponentModel.DataAnnotations;

namespace CommonObjects.Models
{
    /// <summary>
    /// Notification class
    /// </summary>
    public record Notification
    {
        public int Id { get; set; }
        [Required]
        public string Email { get; set; } = null;
        [Required]
        public string OrderId { get; set; } = null;
        [Required]
        public string Type { get; set; } = Constants.NotificationTypeOrderCreated;
        [Required]
        public bool Delivered { get; set; } = false;
        public string ErrorMessage { get; set; } = null;
        public DateTime CreatedAt { get; set; }

    }

    /*1 id(generated)
2 orderId(required)
3 email(required)
4 type(required, example: ORDER_CREATED)
5 delivered(boolean, required)
6 errorMessage(optional)
7 createdAt(optional)*/
}
