using Dtos.Responses;

namespace Services.Abstraction;

public interface IValidationService
{
    public Task<ValidationResponse> ValidateAsync<TRequest>(TRequest request) where TRequest : class;
}