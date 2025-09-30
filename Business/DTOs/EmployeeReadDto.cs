using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTOs
{
    public class EmployeeReadDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int Age { get; set; }
        public string Email { get; set; } = null!;
        public string? Address { get; set; }
        public decimal Salary { get; set; }
        public decimal Balance { get; set; }
        public string? AdditionalInfo { get; set; }
        public List<string> DepartmentNames { get; set; } = new List<string>();
    }
}
