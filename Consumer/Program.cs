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

            channel.ExchangeDeclare(
                exchange: rabbitMQConfig.ExchangeName,
                type: ExchangeType.Direct);

            channel.ExchangeDeclare(
                exchange: rabbitMQConfig.DLExchangeName,
                type: ExchangeType.Fanout);

            var arguments = new Dictionary<string, object>{
                {"x-dead-letter-exchange", rabbitMQConfig.DLExchangeName},
                {"x-message-ttl", 1000},
            };
            channel.QueueDeclare(
                queue: rabbitMQConfig.QueueName,
                arguments: arguments);

            channel.QueueBind(rabbitMQConfig.QueueName, rabbitMQConfig.ExchangeName, "");

            var mainConsumer = new EventingBasicConsumer(channel);
            mainConsumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Main - Recieved new message: {message}");
            };

            //channel.BasicConsume(queue: "mainexchangequeue", consumer: mainConsumer);

            channel.QueueDeclare(queue: rabbitMQConfig.DLXQueueName);
            channel.QueueBind(rabbitMQConfig.DLXQueueName, rabbitMQConfig.DLExchangeName, "");

            var dlxConsumer = new EventingBasicConsumer(channel);
            dlxConsumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"DLX - Recieved new message: {message}");
            };
            channel.BasicConsume(queue: rabbitMQConfig.DLXQueueName, consumer: dlxConsumer);

            Console.WriteLine("Consuming");
            Console.ReadLine();
        }
    }
}
