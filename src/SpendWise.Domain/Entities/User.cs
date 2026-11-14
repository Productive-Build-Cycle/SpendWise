using SpendWise.Domain.Common;
using SpendWise.Domain.Exceptions;

namespace SpendWise.Domain.Entities;

public sealed class User : BaseEntity
{
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;

    public User(Guid id, string firstName, string lastName) : base(id)
    {
        SetFirstName(firstName);
        SetLastName(lastName);
    }

    public void SetFirstName(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName)) throw new DomainValidationException("نام نمی تواند خالی باشد.");
        firstName = firstName.Trim();
        if (firstName.Length < 2 || firstName.Length > 50)
            throw new DomainValidationException("نام باید بین 2 تا 50 کاراکتر باشد.");
        FirstName = firstName;
    }

    public void SetLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName)) throw new DomainValidationException("نام خانوادگی نمی تواند خالی باشد.");
        lastName = lastName.Trim();
        if (lastName.Length < 2 || lastName.Length > 50)
            throw new DomainValidationException("نام خانوادگی باید بین 2 تا 50 کاراکتر باشد.");
        LastName = lastName;
    }
}