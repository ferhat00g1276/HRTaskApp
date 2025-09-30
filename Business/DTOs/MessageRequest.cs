using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTOs
{
    public class MessageRequest
    {
        public string Role { get; set; } // Admin, Manager, Employee
        public string Message { get; set; }
    }
}
