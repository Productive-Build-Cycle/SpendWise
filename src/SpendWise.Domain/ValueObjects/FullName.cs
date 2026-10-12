namespace SpendWise.Domain.ValueObjects;

public sealed class FullName(string firstName, string lastName) : IValueObject
{
  public string FirstName { get; } = DomainValidation.RequireText(firstName, nameof(firstName));
  public string LastName { get; } = DomainValidation.RequireText(lastName, nameof(lastName));

  public bool SameValueAs(IValueObject? other) =>
      other is FullName fullName &&
      FirstName == fullName.FirstName &&
      LastName == fullName.LastName;
}