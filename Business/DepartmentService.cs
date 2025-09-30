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
    public class DepartmentService:IDepartmentService
    {
        private readonly AppDbContext _context;

        public DepartmentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Department>> GetAllAsync()
        {
            return await _context.Departments
                                 .Include(d => d.EmployeeDepartments)
                                 .ToListAsync();
        }

        public async Task<Department?> GetByIdAsync(int id)
        {
            return await _context.Departments
                                 .Include(d => d.EmployeeDepartments) 
                                 .FirstOrDefaultAsync(d => d.Id == id);
        }
        public async Task<List<Employee>> GetEmployeesByDepartmentIdAsync(int departmentId)
        {
            return await _context.EmployeeDepartments
                .Where(ed => ed.DepartmentId == departmentId)
                .Select(ed => ed.Employee)
                .ToListAsync();
        }

        public async Task<Department> CreateAsync(Department department)
        {
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
            return department;
        }

        public async Task UpdateAsync(Department department)
        {
            _context.Entry(department).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department != null)
            {
                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
            }
        }
    }
}
