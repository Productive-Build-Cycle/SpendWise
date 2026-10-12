namespace SpendWise.Domain
{
  /// <summary>Base exception for invariant and business-rule violations.</summary>
  public abstract class DomainException : Exception
  {
      protected DomainException() { }
      protected DomainException(string message) : base(message) { }
      protected DomainException(string message, Exception innerException) : base(message, innerException) { }

      public abstract string ErrorCode { get; }
      public abstract string Title { get; }
      public abstract int HttpStatus { get; }

      public virtual Uri TypeUri => new(
          "https://api.payguard.com/errors/" + ErrorCode.ToLowerInvariant().Replace('_', '-'));
  }
}
