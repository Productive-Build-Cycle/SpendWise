using MyShop.Domain.Exceptions;
using SpendWise.Domain.Common;

namespace SpendWise.Domain.Entities;

public sealed class User : BaseEntity
{
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;

    public User(Guid id, string firstName, string lastName)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Invalid user id", nameof(id));

        Id = id;
        SetFirstName(firstName.Trim());
        SetLastName(lastName.Trim());
    }

    public void SetFirstName(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName)) throw new ValidationException("نام نمی تواند خالی باشد.");
        if (firstName.Length < 2 || firstName.Length > 50)
            throw new ValidationException("نام باید بین 2 تا 50 کاراکتر باشد.");
        FirstName = firstName;
    }

    public void SetLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName)) throw new ValidationException("نام خانوادگی نمی تواند خالی باشد.");
        if (lastName.Length < 2 || lastName.Length > 50)
            throw new ValidationException("نام خانوادگی باید بین 2 تا 50 کاراکتر باشد.");
        LastName = lastName;
    }
}