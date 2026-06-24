using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Models.Database_registers
{
    public class Coil_register
    {
        public string IPAddress { get; set; }

        public string Port { get; set; }

        public string Action { get; set; }

        public DateTime Date { get; set; }
    }
}
