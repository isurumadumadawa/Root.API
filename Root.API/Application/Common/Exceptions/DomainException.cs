namespace Root.API.Application.Common.Exceptions;

public class DomainException : Exception
{
    public string Code { get; }

    public DomainException(string message, string code = "business_rule_violation")
        : base(message)
    {
        Code = code;
    }
}
