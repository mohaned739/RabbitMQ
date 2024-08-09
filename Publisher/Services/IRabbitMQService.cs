namespace Publisher.Services
{
    public interface IRabbitMQService
    {
        void Publish<T>(T message, string routingKey);
    }
}
