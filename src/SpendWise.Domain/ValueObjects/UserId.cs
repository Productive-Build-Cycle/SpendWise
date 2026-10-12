namespace SpendWise.Domain.ValueObjects;

public sealed class UserId(Guid value) : IValueObject
{
  public Guid Value { get; } = value;

  public bool SameValueAs(IValueObject? other) =>
      other is UserId userId && Value == userId.Value;
}