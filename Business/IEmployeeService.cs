using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public interface IEmployeeService
    {
        Task<List<Employee>> GetAllAsync();
        Task<Employee?> GetByIdAsync(int id);
        Task<List<Department>> GetDepartmentsByEmployeeIdAsync(int employeeId);

        Task<Employee> CreateAsync(Employee employee);
        Task UpdateAsync(Employee employee);
        Task DeleteAsync(int id);
        Task<List<Employee>> GetActiveEmployeesAsync();
        Task<List<Employee>> GetActiveEmployeesByDepartmentIdsAsync(List<int> departmentIds);

        Task UpdateEmployeesAsync(List<Employee> employees);

    }
}
