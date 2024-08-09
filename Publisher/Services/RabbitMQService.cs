using RabbitMQ.Client;
using Shared.Configurations;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Channels;

namespace Publisher.Services
{
    public class RabbitMQService : IRabbitMQService
    {
        private readonly RabbitMQConfiguration _configuration;
        private readonly ConnectionFactory _factory;

        public RabbitMQService(RabbitMQConfiguration configuration)
        {
            _configuration = configuration;
            _factory = new ConnectionFactory
            {
                HostName = _configuration.Server,
                UserName = _configuration.Username,
                Password = _configuration.Password
            };
        }

        public void Publish<T>(T message)
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: _configuration.ExchangeName, type: ExchangeType.Headers);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            var properties = channel.CreateBasicProperties();
            properties.Headers = new Dictionary<string, object>() {
                {"name","Mohaned" }
            };

            channel.BasicPublish(exchange: _configuration.ExchangeName,
                                 routingKey: "",
                                 basicProperties: properties,
                                 body: body);
        }
    }
}
