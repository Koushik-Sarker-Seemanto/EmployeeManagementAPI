using AutoMapper;
using Domain.Commands;
using Dtos;
using Entities;

namespace Domain.AutoMapper;

public class AutoMapperConfig : Profile
{
    public AutoMapperConfig()
    {
        CreateMap<EmployeeDto, Employee>().ReverseMap();
        CreateMap<CreateEmployeeCommand, EmployeeDto>().ReverseMap();
    }
}