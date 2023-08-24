using System.Net;

namespace Dtos.Responses;

public class CommandResponse
{
    public CommandResponse()
    {
    }

    public CommandResponse(ValidationResponse validationResult, object result)
    {
        this.ValidationResult = validationResult;
        this.Result = result;
    }

    public ValidationResponse ValidationResult { get; set; }

    public object Result { get; set; }

    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.BadRequest;
}