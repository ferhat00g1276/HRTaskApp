using Business;
using Business.DTOs;
using Domain.Entities;
using Entity;
using Microsoft.AspNetCore.Mvc;
using Business.Mappers;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AutoMapper;
namespace HRTaskApp
{
    [Authorize(Roles = "Admin,Manager")]
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        private readonly IMapper _mapper;

        public DepartmentController(IDepartmentService departmentService,IMapper mapper)
        {
            _departmentService = departmentService;
            _mapper = mapper;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAllDepartments()
        {
            var departments = await _departmentService.GetAllAsync();
            var dtos = departments.Select(d => d.ToReadDto()).ToList();
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartmentById(int id)
        {
            var department = await _departmentService.GetByIdAsync(id);
            if (department == null)
                return NotFound();

            var dto = department.ToReadDto();
            return Ok(dto);
        }

        // GET: api/department/{id}/employees
        [HttpGet("{id}/employees")]
        public async Task<IActionResult> GetEmployeesInDepartment(int id)
        {
            var employees = await _departmentService.GetEmployeesByDepartmentIdAsync(id);
            if (employees == null || !employees.Any())
                return NotFound("No employees found in this department.");

            var employeeDtos = _mapper.Map<List<EmployeeReadDto>>(employees);
            return Ok(employeeDtos);
        }

        // POST: api/department
        [HttpPost]
        public async Task<IActionResult> CreateDepartment(DepartmentCreateDto dto)
        {
            var department = dto.ToEntity();
            await _departmentService.CreateAsync(department);
            return StatusCode(201);
        }

        // PUT: api/department/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, DepartmentUpdateDto dto)
        {
            var department = await _departmentService.GetByIdAsync(id);
            if (department == null)
                return StatusCode(404);

            department.UpdateEntity(dto);
            await _departmentService.UpdateAsync(department);

            return StatusCode(204);
        }

        // DELETE: api/department/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _departmentService.GetByIdAsync(id);
            if (department == null)
                return StatusCode(404);

            await _departmentService.DeleteAsync(id);
            return StatusCode(204);
        }
        [HttpGet("debug-auth")]
        [AllowAnonymous]
        public IActionResult DebugAuth()
        {
            var authHeader = Request.Headers.Authorization.FirstOrDefault();

            return Ok(new
            {
                hasAuthHeader = !string.IsNullOrEmpty(authHeader),
                authHeader = authHeader,
                isAuthenticated = User.Identity?.IsAuthenticated,
                userName = User.Identity?.Name,
                roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray()
            });
        }
    }
}
