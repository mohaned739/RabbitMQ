using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Configurations;
using System.Text;

namespace Producer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
  .SetBasePath(AppContext.BaseDirectory)
  .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
  .Build();

            var rabbitMQConfig = configuration.GetSection(nameof(RabbitMQConfiguration)).Get<RabbitMQConfiguration>();
            var factory = new ConnectionFactory
            {
                HostName = rabbitMQConfig.Server,
                UserName = rabbitMQConfig.Username,
                Password = rabbitMQConfig.Password
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();


            channel.ExchangeDeclare(exchange: rabbitMQConfig.FirstExchangeName, type: ExchangeType.Direct);

            channel.ExchangeDeclare(exchange: rabbitMQConfig.SecondExchangeName, type: ExchangeType.Fanout);

            channel.ExchangeBind(rabbitMQConfig.SecondExchangeName, rabbitMQConfig.FirstExchangeName, "");

            var message = "Hey Code Melters";

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(rabbitMQConfig.FirstExchangeName, "", null, body);

            Console.WriteLine($"Send message: {message}");
            Console.ReadLine();
        }
    }
}
