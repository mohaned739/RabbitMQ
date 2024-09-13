using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Configurations;
using System.Text;

namespace Consumer
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

            channel.ExchangeDeclare(exchange: rabbitMQConfig.SecondExchangeName, type: ExchangeType.Fanout);

            channel.QueueDeclare(rabbitMQConfig.QueueName);

            channel.QueueBind(rabbitMQConfig.QueueName, rabbitMQConfig.SecondExchangeName, "");

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Recieved new message: {message}");
            };

            channel.BasicConsume(queue: rabbitMQConfig.QueueName, autoAck: true, consumer: consumer);

            Console.WriteLine("Consuming");
            Console.ReadLine();
        }
    }
}
