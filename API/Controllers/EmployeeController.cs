
using Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using System;
using DataAccess;
using Business;
using AutoMapper;
using Business.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using API;
using System.IdentityModel.Tokens.Jwt;


namespace HRTaskApp.Controllers
{
    [authorize(roles = "admin,manager")]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;
        private readonly IHubContext<CommunicationHub> _hubContext;
        public EmployeeController(IEmployeeService employeeService, IMapper mapper, IHubContext<CommunicationHub> hubContext)
        {
            _employeeService = employeeService;
            _mapper = mapper;
            _hubContext = hubContext;
        }

        
        // GET: api/employee
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _employeeService.GetAllAsync();
            var dtos = _mapper.Map<List<EmployeeReadDto>>(employees);
            return Ok(dtos);
        }

        // GET: api/employee/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null)
                return NotFound();

            var dto = _mapper.Map<EmployeeReadDto>(employee);
            return Ok(dto);
        }
        [HttpGet("{id}/departments")]
        public async Task<IActionResult> GetDepartmentsForEmployee(int id)
        {
            var departments = await _employeeService.GetDepartmentsByEmployeeIdAsync(id);

            if (departments == null || !departments.Any())
                return NotFound("No departments found for this employee.");

            var departmentDtos = departments.Select(d => new DepartmentReadDto
            {
                Id = d.Id,
                Name = d.Name,
                AdditionalInfo = d.AdditionalInfo
            }).ToList();

            return Ok(departmentDtos);
        }
        [HttpPost]
        public async Task<IActionResult> CreateEmployee(EmployeeCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var employee = _mapper.Map<Employee>(dto);
            await _employeeService.CreateAsync(employee);
            return StatusCode(201);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployeeById(int id, EmployeeUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null)
                return NotFound();

            _mapper.Map(dto, employee);
            await _employeeService.UpdateAsync(employee);
            return StatusCode(204);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeById(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null)
                return StatusCode(404);

            await _employeeService.DeleteAsync(id);
            return StatusCode(204);
        }

        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessageToRole([FromBody] MessageRequest messageRequest)
        {
            if (messageRequest == null || string.IsNullOrEmpty(messageRequest.Role) || string.IsNullOrEmpty(messageRequest.Message))
            {
                return BadRequest("Invalid request");
            }

            // istifadəçinin roluna uyğun qrupa mesaj göndərmək
            await _hubContext.Clients.Group(messageRequest.Role).SendAsync("ReceiveMessage", messageRequest.Message);
            return Ok(new { Status = "Message sent" });
        }
        // admin butun iscilerin maaslarini odeye biler
        [HttpPost("pay-salaries/admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PaySalariesForAllEmployees()
        {

            var adminIdClaim = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            if (adminIdClaim == null)
            {
                return Unauthorized("Employee ID claim missing.");
            }

            int adminId = int.Parse(adminIdClaim);

            var employees = await _employeeService.GetActiveEmployeesAsync();

            if (employees == null || !employees.Any())
            {
                return NotFound("No active employees found.");
            }

            foreach (var employee in employees)
            {
                if (employee.Id == adminId)
                    continue;
                employee.Balance += employee.Salary;  
            }

            await _employeeService.UpdateEmployeesAsync(employees);

            return Ok(new { Status = "Salaries paid to all active employees." });
        }

        //menecer oz departamentindeki iscilerin maaslarini odeye biler
        [HttpPost("pay-salaries/manager")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> PaySalariesForManagerDepartment()
        {
            var managerIdClaim = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            if (managerIdClaim == null)
            {
                return Unauthorized("Employee ID claim missing.");
            }

            int managerId = int.Parse(managerIdClaim);
            var manager = await _employeeService.GetByIdAsync(managerId);

            if (manager == null)
            {
                return Unauthorized("User not found.");
            }

            
            var departmentIds = manager.EmployeeDepartments.Select(ed => ed.DepartmentId).ToList();

            
            var employeesInDepartment = await _employeeService.GetActiveEmployeesByDepartmentIdsAsync(departmentIds);

            if (employeesInDepartment == null || !employeesInDepartment.Any())
            {
                return NotFound("No active employees found in this manager's department.");
            }

            foreach (var employee in employeesInDepartment)
            {
                if (employee.Id == manager.Id)
                    continue;
                employee.Balance += employee.Salary;  
            }

            await _employeeService.UpdateEmployeesAsync(employeesInDepartment);

            return Ok(new { Status = "Salaries paid to all active employees in the manager's department." });
        }

    }
}
