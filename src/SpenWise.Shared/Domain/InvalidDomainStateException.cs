namespace SpendWise.Domain
{
  /// <summary>Illegal domain state transition (HTTP 409).</summary>
  public sealed class InvalidDomainStateException(string message) : DomainException(message)
  {
      public override string ErrorCode => "INVALID_DOMAIN_STATE";
      public override string Title => "Invalid domain state";
      public override int HttpStatus => 409;
  }
}
