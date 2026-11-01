using SpendWise.Domain.Common;
using SpendWise.Domain.Enums;
using SpendWise.Domain.Exceptions;
using SpendWise.Domain.ValueObjects;

namespace SpendWise.Domain.Entities
{
    public class Account:BaseEntity
    {

        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;
        public string AccountName { get; private set; }=string.Empty;
        public AccountType AccountType { get; private set; }
        /// <summary>
        /// The initial financial position recorded for this account  موقعیت مالی اولیه ثبت شده برای این حساب
        /// for bank accounts it represents available funds.
        /// for credit cards it represents outstanding debt.
        /// must be zero or positive.   
        /// </summary>
        public decimal OpeningBalance { get; private set; }
        /// <summary>
        /// the current financial position for this account
        /// for bank accounts it represents available funds.
        /// for credit cards.it represents outstanding debt.
        /// must be zero or positive.
        /// </summary>
        public decimal Balance { get; private set; }
        public Currency Currency { get; private set; } = null!;

        private const string SupportedCurrencyCode = "USD";

        public decimal? CreditLimit { get; private set; }

        public ICollection<Transactions> SourceTransactions { get; private set; }
                                        = new List<Transactions>();
        public ICollection<Transactions> DestinationTransactions { get; private set; }
                                        = new List<Transactions>();

        private Account()
        {
        }

        public Account(Guid userId,
                       string accountName,
                       AccountType accountType,
                      decimal openingBalance,
                       Currency currency,
                       decimal? creditLimit=null)
        {
            if(userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
           
            if (string.IsNullOrWhiteSpace(accountName))
            {
                throw new ArgumentException("Account name cannot be null or empty.", nameof(accountName));
            }
            accountName = accountName.Trim();
            if (accountName.Length > 100)
            {
                throw new ValidationException("Account name cannot exceed 100 characters.");
            }
            if ( openingBalance< 0)
            {
                throw new ValidationException
                    ("Opening balance cannot be negative");
            }

            ArgumentNullException.ThrowIfNull(currency);

            if (currency.Code != SupportedCurrencyCode)
            {
                throw new ValidationException(
                    $"Only {SupportedCurrencyCode} currency is supported in this version.");
            }


            if (!Enum.IsDefined(typeof(AccountType), accountType))
            {
                throw new ArgumentException(
                    "Invalid account type.",
                    nameof(accountType));
            }
           if (accountType == AccountType.CreditCard && creditLimit is null)
            {
                throw new ValidationException(
                    "Credit limit must be provided for credit card accounts." );
            }
           if(accountType==AccountType.CreditCard &&
                creditLimit.HasValue && creditLimit.Value <= 0)
            {
                throw new ValidationException(
                    "Credit limit must be greater than zero for credit card accounts.");
            }
          if (accountType == AccountType.CreditCard &&
                creditLimit.HasValue &&
                openingBalance > creditLimit.Value)
            {
                throw new ValidationException(
                    "Opening balance cannot exceed credit limit for credit card accounts.");
            }
          if (accountType!=AccountType.CreditCard
                && creditLimit.HasValue)
            {
                throw new ValidationException(
                    "Credit limit should only be provided for credit card accounts."  );
            }
            UserId = userId;
            AccountName = accountName;
            AccountType = accountType;
            OpeningBalance = openingBalance;
            Balance = openingBalance;
            Currency = currency;
            CreditLimit = creditLimit;
        }
        
        //..................................................................
        public void Deposit(decimal amount)
        {
            if (AccountType != AccountType.Chequing &&
                AccountType != AccountType.Savings &&
                AccountType != AccountType.DigitalWallet &&
                AccountType != AccountType.Cash &&
                AccountType !=AccountType.Investment)
            {
                throw new ValidationException(
                    "Deposits are not supported for this account type.");
            }
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount),
                    "Deposit amount must be greater than zero.");
            }
          Balance += amount;
        }
        //..................................................................
        public void Withdraw(decimal amount)
        {
            if (AccountType != AccountType.Chequing &&
               AccountType != AccountType.Savings &&
               AccountType != AccountType.DigitalWallet &&
               AccountType != AccountType.Cash &&
               AccountType !=AccountType.Investment)
            {
                throw new ValidationException(
                          "Withdrawals are not supported for this account type");
            }
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount),
                    "Withdrawal amount must be greater than zero.");
            }
            if (amount >Balance)
            {
                throw new ValidationException(
                    "Insufficient funds for this withdrawal.");
            }
            Balance-= amount;
        }
        //..................................................................
        public void PayCreditCard(decimal amount)
        {
            if (AccountType != AccountType.CreditCard)
            {
                throw new ValidationException(
                    "Payments are only allowed for credit card accounts.");
            }
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount),
                    "Payment amount must be greater than zero.");
            }
            if(amount>Balance)
            {
                throw new ValidationException(
                    "Payment amount exceeds the outstanding balance.");
            }
            Balance -= amount;
        }
        //..................................................................
        public void MakeCreditCardPurchase(decimal amount)
        {
            if (AccountType != AccountType.CreditCard)
            {
                throw new ValidationException(
                    "Purchases are only allowed for credit card accounts.");
            }
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount),
                    "Purchase amount must be greater than zero.");
            }
           
            if (amount+Balance>CreditLimit!.Value)
            {
                throw new ValidationException(
                    "Purchase amount exceeds the credit limit.");
            }
            Balance += amount;
        }
    }
}
