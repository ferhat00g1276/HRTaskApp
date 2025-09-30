using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Business.DTOs
{
    public class EmployeeCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = null!;

        [Required]
        [Range(18, 65, ErrorMessage = "Age must be between 18 and 65")]
        public int Age { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        public string? Address { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Salary must be positive")]
        public decimal Salary { get; set; }

        public string? AdditionalInfo { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Balance must be positive")]
        public decimal Balance { get; set; }

        public List<int> DepartmentIds { get; set; } = new List<int>();
    }
}
