using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Configurations;
using System.Text;

namespace Server
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

            
            channel.QueueDeclare(queue: rabbitMQConfig.QueueName, exclusive: false);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, args) =>
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Server: Recieved Request: {message}\n{args.BasicProperties.CorrelationId}");
                var replyMessage = $"Reply: Hope you are enjoying this playlist\n{args.BasicProperties.CorrelationId}";
                var replyBody = Encoding.UTF8.GetBytes(replyMessage);
                channel.BasicPublish("", args.BasicProperties.ReplyTo, null, replyBody);

            };

            channel.BasicConsume(queue: rabbitMQConfig.QueueName, autoAck: true, consumer: consumer);

            Console.ReadLine();
        }
    }
}
