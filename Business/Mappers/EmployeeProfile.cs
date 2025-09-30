using AutoMapper;
using Business.DTOs;
using Entity;
using System.Collections.Generic;
using System.Linq;

namespace Business.Mappers
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            // Entity -> ReadDto
            CreateMap<Employee, EmployeeReadDto>()
                .ForMember(dest => dest.DepartmentNames,
                           opt => opt.MapFrom(src =>
                               src.EmployeeDepartments != null
                               ? src.EmployeeDepartments.Select(ed => ed.Department.Name).ToList()
                               : new List<string>()))
                .ForMember(dest => dest.Balance, 
                           opt => opt.MapFrom(src => src.Balance));

            // CreateDto -> Entity
            CreateMap<EmployeeCreateDto, Employee>()
                .ForMember(dest => dest.EmployeeDepartments,
                           opt => opt.MapFrom(src =>
                               src.DepartmentIds.Select(id => new EmployeeDepartment
                               {
                                   DepartmentId = id
                               }).ToList()))
                .ForMember(dest => dest.Balance,  
                           opt => opt.MapFrom(src => src.Balance));  

            // UpdateDto -> Entity
            CreateMap<EmployeeUpdateDto, Employee>()
                .ForMember(dest => dest.EmployeeDepartments,
                           opt => opt.MapFrom(src =>
                               src.DepartmentIds.Select(id => new EmployeeDepartment
                               {
                                   DepartmentId = id
                               }).ToList()))
                .ForMember(dest => dest.IsActive,
                           opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.Balance,  
                           opt => opt.MapFrom(src => src.Balance));
        }
    }
}
