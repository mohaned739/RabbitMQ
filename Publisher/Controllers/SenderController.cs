using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Publisher.Services;
using Shared.Configurations;
using Shared.Messages;

namespace Publisher.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SenderController : ControllerBase
    {
        private readonly IRabbitMQService _rabbitMQService;

        public SenderController(IRabbitMQService rabbitMQService)
        {   
            _rabbitMQService = rabbitMQService;
        }

        [HttpPost]
        public async Task<ActionResult> SendMessage([FromBody] CompetingConsumersMessage message)
        {
            try
            {
                _rabbitMQService.Publish(message);
                return Ok("Message sent successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
