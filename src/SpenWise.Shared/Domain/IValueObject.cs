namespace SpendWise.Domain
{
  /// <summary>Contract for comparing domain values rather than object identity.</summary>
  public interface IValueObject
  {
      bool SameValueAs(IValueObject? other);
  }
}
