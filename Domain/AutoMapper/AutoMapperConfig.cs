using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Domain.Commands;
using Dtos;
using Entities;

namespace Domain.AutoMapper;

[ExcludeFromCodeCoverage]
public class AutoMapperConfig : Profile
{
    public AutoMapperConfig()
    {
        CreateMap<Employee, EmployeeDto>().ReverseMap();
        CreateMap<CreateEmployeeCommand, EmployeeDto>().ReverseMap();
        CreateMap<UpdateEmployeeCommand, EmployeeDto>().ReverseMap();
    }
}