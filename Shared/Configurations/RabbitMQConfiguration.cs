using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Configurations
{
    public class RabbitMQConfiguration
    {
        public string Server { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string QueueName { get; set; }
        public string FirstExchangeName { get; set; }
        public string SecondExchangeName { get; set; }
    }
}
