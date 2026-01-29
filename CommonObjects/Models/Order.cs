using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CommonObjects.Models
{
    /// <summary>
    /// Order class will be used for both OrderDto and Order Entity
    /// </summary>
    public class Order
    {
        [Key] // Database Primary Key
        public int Id { get; set; }
        [Required]
        public string CustomerEmail { get; set; } = null;
        [Required]
        public string ProductCode { get; set; } = null;
        [Required]
        public int Quantity { get; set; }
        public string Status { get; set; } = null;
        [JsonIgnore]
        public DateTimeOffset CreatedAt { get; set; }
        [NotMapped]
        public DateTime CreatedAtDateTIme { get; set; }
    }

/*
         * 1 id (generated)
        2 customerEmail (required)
        3 productCode (required)
        4 quantity (required, positive)
        5 status (optional, default CREATED)
        6 createdAt (optional)
         */
}
