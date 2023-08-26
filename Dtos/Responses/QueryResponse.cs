using System.Net;

namespace Dtos.Responses;

public class QueryResponse<TResult>
{
    public QueryResponse()
    {
    }

    public QueryResponse(ValidationResponse validationResult, TResult result)
    {
        this.ValidationResult = validationResult;
        this.Result = result;
    }

    public ValidationResponse ValidationResult { get; set; }

    public TResult Result { get; set; }
    
    public int Count { get; set; }

    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.BadRequest;
}