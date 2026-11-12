namespace SpendWise.Application.Common.Exceptions;

public sealed class DomainRuleViolationException : Exception
{
    public DomainRuleViolationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}