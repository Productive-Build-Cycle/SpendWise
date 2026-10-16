namespace SpendWise.Application.Common.Exceptions;

public sealed class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException() : base("Invalid username or password.")
    {
    }
}