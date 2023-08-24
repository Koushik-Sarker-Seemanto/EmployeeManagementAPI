namespace Dtos.Responses;

public class ValidationResponse
{
    public List<ValidationError> Errors { get; set; }

    public ValidationResponse()
    {
        this.Errors = new List<ValidationError>();
    }

    public ValidationResponse(List<ValidationError> errors)
    {
        this.Errors = errors;
    }

    /// <summary>Adds the error.</summary>
    /// <param name="errorMessage">The error message.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="errorCode">The error code.</param>
    /// <param name="resourceName">Name of the resource.</param>
    public void AddError(string errorMessage, string propertyName = "", string errorCode = "", string resourceName = "")
    {
        this.Errors.Add(new ValidationError { ErrorMessage = errorMessage, PropertyName = propertyName, ErrorCode = errorCode, ResourceName = resourceName });
    }
    
    public bool IsValid => !this.Errors.Any();
}