namespace Domain.Queries;

public class BaseQuery
{
    public string? CorrelationId { get; set; }
    public int PageNo { get; set; } = 0;
    public int PageSize { get; set; } = 10;
}