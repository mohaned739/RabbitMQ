using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Configurations;

namespace Consumer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                });

            builder.ConfigureServices((hostContext, services) =>
            {
                var rabbitMqConfiguration = hostContext.Configuration.GetSection(nameof(RabbitMQConfiguration)).Get<RabbitMQConfiguration>();
                services.AddMassTransit(x =>
                {
                    x.AddConsumer<DemoConsumer>()
                        .Endpoint(e => e.Name = rabbitMqConfiguration.QueueName);
                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host(rabbitMqConfiguration.Server, h =>
                        {
                            h.Username(rabbitMqConfiguration.Username);
                            h.Password(rabbitMqConfiguration.Password);
                        });
                        cfg.ConfigureEndpoints(context);
                        cfg.Exclusive = false;
                        cfg.ConcurrentMessageLimit = 1;

                    });
                });
            });


            builder.Build().Run();
        }
    }
}
