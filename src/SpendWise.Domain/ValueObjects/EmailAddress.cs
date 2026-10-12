namespace SpendWise.Domain.ValueObjects;

public sealed class EmailAddress(string value) : IValueObject
{
  public string Value { get; } = DomainValidation.RequireText(value, nameof(value));

  public bool SameValueAs(IValueObject? other) =>
      other is EmailAddress emailAddress && Value == emailAddress.Value;
}