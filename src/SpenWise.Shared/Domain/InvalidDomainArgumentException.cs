namespace SpendWise.Domain
{
  /// <summary>Invalid value or command input (HTTP 400).</summary>
  public sealed class InvalidDomainArgumentException(string message) : DomainException(message)
  {
      public override string ErrorCode => "INVALID_DOMAIN_ARGUMENT";
      public override string Title => "Invalid domain argument";
      public override int HttpStatus => 400;
  }
}
