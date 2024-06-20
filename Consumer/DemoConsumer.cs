using MassTransit;
using Shared.Messages;

namespace Consumer
{
    internal class DemoConsumer : IConsumer<DemoMessage>
    {
        public async Task Consume(ConsumeContext<DemoMessage> context)
        {
            var message = context.Message;
            Console.WriteLine($"From: {message.From}\nTo: {message.To}\nSubject:{message.Title}" +
                $"\n{message.Content}");
        }
    }
}
