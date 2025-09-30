using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Business.DTOs;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Entity;
using System;
namespace HRTaskApp
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        public AuthController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            var employee = _context.Employees
                .FirstOrDefault(e => e.FirstName == dto.Username && e.Password == dto.Password);

            if (employee == null)
                return Unauthorized("Yanlış FirstName və ya Password");

            var role = employee.Role;

            var claims = new[]
            {
        new Claim(ClaimTypes.Name, employee.FirstName),
        new Claim(ClaimTypes.Role, role),
        new Claim(JwtRegisteredClaimNames.Sub, employee.Id.ToString()), 
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), 
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                role = role 
            });
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Email və ya FirstName ilə artıq istifadəçi var yoxlanışı
            var existingEmployee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Email == dto.Email);
            if (existingEmployee != null)
                return BadRequest("Bu email artıq istifadə olunub.");

            // Yeni employee entity yaratmaq
            var employee = new Employee
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = dto.Password,

                Address = dto.Address,
                Salary = dto.Salary ?? 0,
                AdditionalInfo = dto.AdditionalInfo,
                IsActive = true
            };

            // Role təyini FirstName-ə əsasən
            if (dto.FirstName.ToLower() == "admin")
                employee.Role = "Admin";
            else if (dto.FirstName.ToLower() == "manager")
                employee.Role = "Manager";
            else
                employee.Role = "Employee";

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            if (dto.DepartmentId != null)
            {
                var employeeDepartment = new EmployeeDepartment
                {
                    EmployeeId = employee.Id,
                    DepartmentId = (int)(dto.DepartmentId)
                };


                _context.EmployeeDepartments.Add(employeeDepartment);
                await _context.SaveChangesAsync();
            }
            return StatusCode(201, new { message = "User registered successfully", role = employee.Role });
        }













    }
}
