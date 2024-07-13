using Shared.Messages;

namespace Publisher.Services
{
    public interface IRabbitMQService
    {
        void Publish(CompetingConsumersMessage message);
    }
}
