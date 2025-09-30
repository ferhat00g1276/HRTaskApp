using AutoMapper;
using Business.DTOs;
using Domain.Entities;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Mappers
{
    public static class DepartmentMapper
    {
        //public static DepartmentReadDto ToReadDto(this Department department)
        //{
        //    return new DepartmentReadDto
        //    {
        //        Id = department.Id,
        //        Name = department.Name,
        //        AdditionalInfo = department.AdditionalInfo,
        //        //EmployeeCount = department.Employees.Count,
        //        //Employees = department.Employees.Select(e => new EmployeeReadDto
        //        //{
        //        //    Id = e.Id,
        //        //    FirstName = e.FirstName,
        //        //    LastName = e.LastName,
        //        //    Age = e.Age,
        //        //    Email = e.Email,
        //        //    Address = e.Address,
        //        //    Salary = e.Salary,
        //        //    AdditionalInfo = e.AdditionalInfo,

        //        //    DepartmentName = department.Name
        //        //}).ToList()
        //    };
        //}
        public static DepartmentReadDto ToReadDto(this Department department)
        {
            return new DepartmentReadDto
            {
                Id = department.Id,
                Name = department.Name,
                AdditionalInfo = department.AdditionalInfo,
                EmployeeCount = department.EmployeeDepartments?.Count ?? 0
            };
        }
        public static Department ToEntity(this DepartmentCreateDto dto)
        {
            return new Department
            {
                Name = dto.Name,
                AdditionalInfo = dto.AdditionalInfo
            };
        }

        public static void UpdateEntity(this Department department, DepartmentUpdateDto dto)
        {
            department.Name = dto.Name;
            department.AdditionalInfo = dto.AdditionalInfo;
        }
    }
}
