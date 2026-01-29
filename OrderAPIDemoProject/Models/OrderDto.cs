using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;

namespace OrderAPIDemoProject.Models
{
    public class OrderDto
    {
        //public int Id { get; set; }
        public string CustomerEmail { get; set; }
        public string ProductCode { get; set; }
        public int Quantity { get; set; }
        //public string Status { get; set; }
        //public DateTime CreatedAt { get; set; }
        /*
         * 1 id (generated)
        2 customerEmail (required)
        3 productCode (required)
        4 quantity (required, positive)
        5 status (optional, default CREATED)
        6 createdAt (optional)
         */
    }

}
