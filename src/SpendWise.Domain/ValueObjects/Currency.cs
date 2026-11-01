


namespace SpendWise.Domain.ValueObjects;

public sealed class Currency : IEquatable<Currency>
{
    public string Code { get; }
    public Currency(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException(
                "Currency code cannot be null or empty.",
                nameof(code));
        }

        Code = code.Trim().ToUpperInvariant();

        if (Code.Length != 3 || Code.Any(c => c < 'A' || c > 'Z'))
        {
            throw new ArgumentException(
                "Currency code must contain exactly three English letters.",
                nameof(code));
        }
    }
    public bool Equals(Currency? other)
    {
        return other is not null &&
               Code == other.Code;
    }

    public override bool Equals(object? obj)
    {
        return obj is Currency other &&
               Equals(other);
    }
    public override int GetHashCode()
    {
        return Code.GetHashCode();
    }
    public static bool operator ==(Currency? left, Currency? right)
    {
        return object.Equals(left, right);
    }
    public static bool operator !=(Currency? left, Currency? right)
    {
        return !object.Equals(left, right);
    }
}
