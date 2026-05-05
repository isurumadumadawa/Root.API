namespace Root.API.Contracts.Responses;

public class ValidationErrorItem
{
    public string Field { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
}
