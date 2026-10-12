namespace SpendWise.Domain.ValueObjects;

public sealed class PasswordHash(string value) : IValueObject
{
  public string Value { get; } = DomainValidation.RequireText(value, nameof(value));

  public bool SameValueAs(IValueObject? other) =>
      other is PasswordHash passwordHash && Value == passwordHash.Value;
}