using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CommonObjects.Models
{
    /// <summary>
    /// RabbitMQConfig configurations record class
    /// </summary>
    public record RabbitMQConfig
    {
        public const string SectionName = "RabbitMQ"; // Define a constant for the section name

        public string HostName { get; init; } = "localhost";
        public string UserName { get; init; } = "guest";
        public string Password { get; init; } = "guest";
        public string VirtualHost { get; init; } = "/";
        // Add other properties like AutomaticRecoveryEnabled, RequestedHeartbeat, etc. as needed
    }

}
