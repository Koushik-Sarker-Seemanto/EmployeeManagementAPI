using System.Diagnostics.CodeAnalysis;
using Domain.CommandHandlers;
using Domain.Commands;
using Domain.Queries;
using Domain.QueryHandlers;
using Domain.Validators.CommandValidators;
using Domain.Validators.QueryValidators;
using Dtos;
using Dtos.Responses;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Domain.Extension;

[ExcludeFromCodeCoverage]
public static class DomainExtensions
{
    public static IServiceCollection RegisterDomainExtensions(this IServiceCollection services)
    {
        // Register Handlers
        services.AddScoped<IRequestHandler<CreateEmployeeCommand, CommandResponse>, CreateEmployeeCommandHandler>();
        services.AddScoped<IRequestHandler<UpdateEmployeeCommand, CommandResponse>, UpdateEmployeeCommandHandler>();
        services.AddScoped<IRequestHandler<DeleteEmployeeQuery, QueryResponse<EmployeeDto>>, DeleteEmployeeQueryHandler>();
        services.AddScoped<IRequestHandler<GetEmployeesQuery, QueryResponse<List<EmployeeDto>>>, GetEmployeesQueryHandler>();
        services.AddScoped<IRequestHandler<GetEmployeeByIdQuery, QueryResponse<EmployeeDto>>, GetEmployeeByIdQueryHandler>();
        
        //Register Validators
        services.AddScoped<IValidator<CreateEmployeeCommand>, CreateEmployeeCommandValidator>();
        services.AddScoped<IValidator<UpdateEmployeeCommand>, UpdateEmployeeCommandValidator>();
        services.AddScoped<IValidator<DeleteEmployeeQuery>, DeleteEmployeeQueryValidator>();
        services.AddScoped<IValidator<GetEmployeesQuery>, GetEmployeesQueryValidator>();
        services.AddScoped<IValidator<GetEmployeeByIdQuery>, GetEmployeeByIdQueryValidator>();
        
        return services;
    }
}