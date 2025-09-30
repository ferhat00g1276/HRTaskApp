using AutoMapper;
using Business.DTOs;
using Entity;

public class DepartmentProfile : Profile
{
    public DepartmentProfile()
    {
        CreateMap<Department, DepartmentReadDto>()
            .ForMember(dest => dest.EmployeeCount,
                       opt => opt.MapFrom(src => src.EmployeeDepartments.Count));
    }
}
