
namespace Entity
{
    public class Employee
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int Age { get; set; }
        public string Email { get; set; } = null!;
        public string? Address { get; set; }
        public string Password { get; set; } = null!;

        public decimal Salary { get; set; }
        public decimal Balance { get; set; }
        public string? AdditionalInfo { get; set; }

        public bool IsActive { get; set; } = true;
        public string Role { get; set; } = "Employee";

        public ICollection<EmployeeDepartment> EmployeeDepartments { get; set; } = new List<EmployeeDepartment>();
    }
}
