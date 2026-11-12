using SpendWise.Domain.Exceptions;

namespace SpendWise.Domain.ValueObjects;

public sealed class Money : IEquatable<Money>
{
    public decimal Amount { get; }
    public Currency Currency { get; }

    private const decimal MaxAmount = 9_999_999_999_999_999.99m;

    public Money(decimal amount, Currency currency)
    {
        ArgumentNullException.ThrowIfNull(currency);

        if (amount < 0)
            throw new DomainValidationException("Money amount cannot be negative.");

        if (amount > MaxAmount)
            throw new DomainValidationException("Money amount exceeds the maximum supported value.");

        if (decimal.Round(amount, 2) != amount)
            throw new DomainValidationException("Money amount cannot have more than two decimal places.");

        Amount = amount;
        Currency = currency;
    }

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);

        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);

        if (other.Amount > Amount)
        {
            throw new DomainValidationException(
                "Money amount cannot be negative.");
        }

        return new Money(Amount - other.Amount, Currency);
    }

    private void EnsureSameCurrency(Money other)
    {
        ArgumentNullException.ThrowIfNull(other);

        if (Currency != other.Currency)
        {
            throw new DomainValidationException(
                "Cannot perform an operation with different currencies.");
        }
    }

    public bool Equals(Money? other)
    {
        return other is not null &&
               Amount == other.Amount &&
               Currency == other.Currency;
    }

    public override bool Equals(object? obj)
    {
        return obj is Money other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Amount, Currency);
    }

    public static bool operator ==(Money? left, Money? right)
    {
        return object.Equals(left, right);
    }

    public static bool operator !=(Money? left, Money? right)
    {
        return !object.Equals(left, right);
    }
}