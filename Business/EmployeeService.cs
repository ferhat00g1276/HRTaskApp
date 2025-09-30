using DataAccess;
using Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class EmployeeService:IEmployeeService
    {
        private readonly AppDbContext _context;

        public EmployeeService(AppDbContext context)
        {
            _context = context;
        }

       
        public async Task<List<Employee>> GetAllAsync()
        {
            return await _context.Employees
                                 .Include(e => e.EmployeeDepartments)
                                    .ThenInclude(ed => ed.Department)
                                 .ToListAsync();
        }
        
        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _context.Employees
                                 .Include(e => e.EmployeeDepartments)
                                    .ThenInclude(ed => ed.Department)
                                 .FirstOrDefaultAsync(e => e.Id == id);
        }
        public async Task<List<Department>> GetDepartmentsByEmployeeIdAsync(int employeeId)
        {
            return await _context.EmployeeDepartments
                                 .Where(ed => ed.EmployeeId == employeeId)
                                 .Include(ed => ed.Department)
                                 .Select(ed => ed.Department)
                                 .ToListAsync();
        }

        public async Task<Employee> CreateAsync(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task UpdateAsync(Employee employee)
        {
            _context.Entry(employee).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<Employee>> GetActiveEmployeesAsync()
        {
            return await _context.Employees
                .Where(e => e.IsActive)  
                .ToListAsync();
        }
        public async Task<List<Employee>> GetActiveEmployeesByDepartmentIdsAsync(List<int> departmentIds)
        {
            return await _context.Employees
                .Where(e => e.IsActive && e.EmployeeDepartments
                    .Any(ed => departmentIds.Contains(ed.DepartmentId))) 
                .ToListAsync();
        }
        public async Task UpdateEmployeesAsync(List<Employee> employees)
        {
            _context.Employees.UpdateRange(employees);  
            await _context.SaveChangesAsync();         
        }


    }
}
