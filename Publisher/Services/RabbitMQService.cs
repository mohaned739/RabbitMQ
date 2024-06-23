using RabbitMQ.Client;
using Shared.Configurations;
using System.Text.Json;
using System.Text;

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

            channel.QueueDeclare(queue: _configuration.QueueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            channel.BasicPublish(exchange: "",
                                 routingKey: _configuration.QueueName,
                                 basicProperties: null,
                                 body: body);
        }
    }
}
