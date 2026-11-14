namespace SpendWise.Application.Common.Exceptions;

public sealed class UnauthenticatedUserException : Exception
{
    public UnauthenticatedUserException() : base("Authenticated user could not be resolved.")
    {
    }
}