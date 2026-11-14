namespace SpendWise.Application.Interfaces;

public interface ICurrentUser
{
    Guid UserId { get; }
}