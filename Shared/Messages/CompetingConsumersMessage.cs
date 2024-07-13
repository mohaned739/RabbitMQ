using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Messages
{
    public class CompetingConsumersMessage
    {
        public int Id { get; set; }
        public string Content { get; set; }
    }
}
