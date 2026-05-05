namespace Root.API.Contracts.Responses;

public class ErrorResponse
{
    public string Code { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string TraceId { get; init; } = string.Empty;
    public IReadOnlyList<ValidationErrorItem>? Errors { get; init; }
}
