using SpendWise.Domain.Common;
using SpendWise.Domain.Enums;


namespace SpendWise.Domain.Entities
{
    public class Account:BaseEntity
    {

        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;
        public string AccountName { get; private set; }=string.Empty;
        public AccountType AccountType { get; private set; }
        public decimal OpeningBalance { get; private set; }
        public decimal Balance { get; private set; }
        public string Currency { get; private set; } = string.Empty;

        private Account()
        {
        }

        public Account(Guid userId,
                       string accountName,
                       AccountType accountType,
                      decimal openingBalance,
                       string currency)
        {
            if(userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            if (string.IsNullOrWhiteSpace(accountName))
            {
                throw new ArgumentException("Account name cannot be null or empty.", nameof(accountName));
            }
            if ( openingBalance< 0)
            {
                throw new ArgumentException("Balance cannot be negative.", nameof(openingBalance));
            }
            if(string.IsNullOrWhiteSpace(currency))
            {
                throw new ArgumentException("Currency cannot be null or empty.", nameof(currency));
            }
            if (!Enum.IsDefined(typeof(AccountType), accountType))
            {
                throw new ArgumentException(
                    "Invalid account type.",
                    nameof(accountType));
            }
            UserId = userId;
            AccountName = accountName.Trim();
            AccountType = accountType;
            OpeningBalance = openingBalance;
            Balance = openingBalance;
            Currency = currency.Trim().ToUpperInvariant();
        }

    }
}
