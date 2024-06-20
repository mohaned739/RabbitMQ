
using MassTransit;
using Shared.Configurations;

namespace Publisher
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var rabbitMqConfig = builder.Configuration.GetSection(nameof(RabbitMQConfiguration)).Get<RabbitMQConfiguration>();
            builder.Services.AddMassTransit(options =>
            {
                options.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitMqConfig.Server, h =>
                    {
                        h.Username(rabbitMqConfig.Username);
                        h.Password(rabbitMqConfig.Password);
                    });
                    cfg.ConfigureEndpoints(context);
                    cfg.Exclusive = false;
                    cfg.Durable = true;
                    cfg.ConcurrentMessageLimit = 1;
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
