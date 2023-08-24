using Domain.CommandHandlers;
using Domain.Commands;
using Domain.Validators.CommandValidators;
using Dtos.Responses;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Domain.Extension;

public static class DomainExtensions
{
    public static IServiceCollection RegisterDomainExtensions(this IServiceCollection services)
    {
        // Register Handlers
        services.AddScoped<IRequestHandler<CreateEmployeeCommand, CommandResponse>, CreateEmployeeCommandHandler>();
        services.AddScoped<IRequestHandler<UpdateEmployeeCommand, CommandResponse>, UpdateEmployeeCommandHandler>();
        
        //Register Validators
        services.AddScoped<IValidator<CreateEmployeeCommand>, CreateEmployeeCommandValidator>();
        services.AddScoped<IValidator<UpdateEmployeeCommand>, UpdateEmployeeCommandValidator>();
        
        return services;
    }
}