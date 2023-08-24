using Microsoft.Extensions.DependencyInjection;
using Services.Abstraction;

namespace Services.Extension;

public static class ServiceExtensions
{
    public static IServiceCollection RegisterServiceExtensions(this IServiceCollection services)
    {
        services.AddScoped<IValidationService, ValidationService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        return services;
    }
}