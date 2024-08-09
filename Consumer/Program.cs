﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Configurations;
using System.Text;

namespace Consumer1
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

            channel.ExchangeDeclare(exchange: rabbitMQConfig.ExchangeName, ExchangeType.Topic);

            var queueName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queue: queueName, exchange: rabbitMQConfig.ExchangeName, routingKey: rabbitMQConfig.BindingKey);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, args) =>
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"First Consumer: Message Recieved: {message}");
            };

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

            Console.ReadLine();
        }
    }
}
