using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTOs
{
    public class DepartmentUpdateDto
    {
        [Required]
        public int Id { get; set; }   

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        public string? AdditionalInfo { get; set; }
    }
}
