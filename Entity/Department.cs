using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Department
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public string? AdditionalInfo { get; set; }


        public ICollection<EmployeeDepartment> EmployeeDepartments { get; set; } = new List<EmployeeDepartment>();


    }
}
