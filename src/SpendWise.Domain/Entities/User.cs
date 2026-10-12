
using SpendWise.Domain.ValueObjects;

namespace SpendWise.Domain.Entities;

public class User : AggregateRoot<UserId, IDomainEvent>
{
    private User(
        UserId userId,
        FullName fullName,
        string userName,
        EmailAddress email,
        PasswordHash passwordHash) : base(userId)
    {
        FullName = fullName;
        UserName = userName;
        Email = email;
        PasswordHash = passwordHash;
    }

    public UserId UserId => Id;

    public FullName FullName { get; private set; }
    public string UserName { get; private set; }
    public EmailAddress Email { get; private set; }
    public PasswordHash PasswordHash { get; private set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? ArchivedAt { get; set; }

    public static User Create(
        UserId userId,
        FullName fullName,
        string userName,
        EmailAddress email,
        PasswordHash passwordHash)
    {
        var user = new User(
            userId,
            fullName,
            DomainValidation.RequireText(userName, nameof(userName)),
            email,
            passwordHash);

        user.CreatedAt = DateTime.UtcNow;
        return user;
    }
}
