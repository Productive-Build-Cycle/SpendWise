namespace SpendWise.Application.Common.Exceptions;

public sealed class DomainOperationException : Exception
{
    public DomainOperationException(Exception innerException) : base("An unexpected domain operation error occurred.",
        innerException)
    {
    }
}