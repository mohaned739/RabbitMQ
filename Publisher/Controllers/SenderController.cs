using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Configurations;
using Shared.Messages;

namespace Publisher.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SenderController : ControllerBase
    {
        private readonly IBus _bus;
        private readonly IConfiguration _config;

        public SenderController(IBus bus, IConfiguration config)
        {
            _bus = bus;
            _config = config;
        }

        [HttpPost]
        public async Task<ActionResult> SendMessage([FromBody] DemoMessage message)
        {
            try
            {
                var rabbitMqConfiguration = _config.GetSection(nameof(RabbitMQConfiguration)).Get<RabbitMQConfiguration>();
                var endpoint = await _bus.GetSendEndpoint(new Uri($"exchange:{rabbitMqConfiguration.QueueName}"));
                await endpoint.Send(message);
                return Ok("Message sent successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
