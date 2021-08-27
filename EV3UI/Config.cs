using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV3UI
{
    class Config
    {
        public string LogFileName { get; set; }
        public string RabbitUserName { get; set; }
        public string RabbitPassword { get; set; }
        public string RabbitVHost { get; set; }
        public string RabbitHost { get; set; }
        public int RabbitPort { get; set; }
    }
}
