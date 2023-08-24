using Dtos.Responses;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Services.Abstraction;

namespace Services;

public class ValidationService : IValidationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ValidationService> _logger;
    public ValidationService(ILogger<ValidationService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task<ValidationResponse> ValidateAsync<TRequest>(TRequest request) where TRequest : class
    {
        try
        {
            var validatorType = typeof(IValidator<>).MakeGenericType(request.GetType());

            var validator = _serviceProvider.GetService(validatorType) as IValidator;
            if (validator == null)
            {
                _logger.LogWarning($"No validator found for {request.GetType().Name}");
                throw new Exception($"No validator found for {request.GetType().Name}");
            }


            var validationContext = new ValidationContext<TRequest>(request);
            var validationResult = await validator.ValidateAsync(validationContext);

            return BuildValidationResponse(validationResult);
        }
        catch (Exception e)
        {
            _logger.LogError($"Exception occurred while executing ValidateAsync. Exception: {e.Message}");
            throw;
        }
    }
    
    
    private static ValidationResponse BuildValidationResponse(
        ValidationResult validationResult)
    {
        return new ValidationResponse
        {
            Errors = validationResult.Errors.Select(failure => new ValidationError
            {
                PropertyName = failure.PropertyName,
                ErrorMessage = failure.ErrorMessage,
                ErrorCode = failure.ErrorCode,
            }).ToList(),
        };
    }
}