namespace SpendWise.Domain
{
  /// <summary>Guards domain boundaries using domain-specific errors.</summary>
  public static class DomainValidation
  {
      public static T RequireNonNull<T>(T? value, string fieldName) where T : notnull =>
          value is null
              ? throw new InvalidDomainArgumentException($"{fieldName} cannot be null")
              : value;

      public static string RequireText(string? value, string fieldName) =>
          string.IsNullOrWhiteSpace(value)
              ? throw new InvalidDomainArgumentException($"{fieldName} cannot be blank")
              : value;
  }
}
