using SpendWise.Domain.Common;
using SpendWise.Domain.Enums;
using SpendWise.Domain.Exceptions;
using SpendWise.Domain.ValueObjects;
using System.Net;

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
        public Money OpeningBalance { get; private set; }
        /// <summary>
        /// the current financial position for this account
        /// for bank accounts it represents available funds.
        /// for credit cards.it represents outstanding debt.
        /// must be zero or positive.
        /// </summary>
        public Money Balance { get; private set; }

        private const string SupportedCurrencyCode = "USD";

        public Money? CreditLimit { get; private set; }

        public ICollection<Transactions> SourceTransactions { get; private set; }
                                        = new List<Transactions>();
        public ICollection<Transactions> DestinationTransactions { get; private set; }
                                        = new List<Transactions>();

        private Account()
        {
            OpeningBalance = null!;
            Balance = null!;
        }

        public Account(Guid userId,
                       string accountName,
                       AccountType accountType,
                       Money openingBalance,
                       Money? creditLimit=null)
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

            ArgumentNullException.ThrowIfNull(openingBalance);
            if (openingBalance.Amount < 0)
            {
                throw new ValidationException(
                    "Opening balance cannot be negative.");
            }

           if(openingBalance.Currency.Code != SupportedCurrencyCode)
            {
                throw new ValidationException(
                    $"only {SupportedCurrencyCode} currency is supported in this version.");
            }


            if (!Enum.IsDefined(typeof(AccountType), accountType))
            {
                throw new ArgumentException(
                    "Invalid account type.",
                    nameof(accountType));
            }
            if (accountType != AccountType.CreditCard && 
                 creditLimit is not null)
            {
                throw new ValidationException(
                    "creditLimit should only be  provided for credit card accounts.");
            }
            if (accountType == AccountType.CreditCard && creditLimit is null)
            {
                throw new ValidationException(
                    "Credit limit must be provided for credit card accounts." );
            }
           if(creditLimit is not null &&
                creditLimit.Currency.Code != openingBalance.Currency.Code)
            {
                throw new ValidationException(
                    $"Credit limit must have the same currency as the opening balance.");
            }
          if (accountType==AccountType.CreditCard &&
                creditLimit is not null &&
                 openingBalance.Amount> creditLimit.Amount)
            {
                throw new ValidationException(
                    "Opening balance cannot exceed the credit limit for credit card accounts.");
            }
          if(accountType==AccountType.CreditCard &&
                creditLimit is not null &&
                creditLimit.Amount <= 0)
            {
                throw new ValidationException(
                    "Credit limit must be a positive value for credit card accounts.");
            }
            UserId = userId;
            AccountName = accountName;
            AccountType = accountType;
            OpeningBalance = openingBalance;
            Balance = openingBalance;
           
            CreditLimit = creditLimit;
        }
        
        //..................................................................
        public void Deposit(Money deposit)
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
            ArgumentNullException.ThrowIfNull(deposit);
            if (deposit.Amount <= 0)
            {
                throw new ValidationException(
                    "Deposit amount must be greater than zero.");
            }
            if(deposit.Currency != Balance.Currency)
            {
                throw new ValidationException(
                    "Deposit currency must match the account currency.");
            }
          Balance = Balance.Add(deposit);
        }
        //..................................................................
        public void Withdraw(Money withdrawal)
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
            ArgumentNullException.ThrowIfNull(withdrawal);
            if (withdrawal.Amount <= 0)
            {
                throw new ValidationException(
                    "Withdrawal amount must be greater than zero.");
            }
            if (withdrawal.Currency != Balance.Currency)
            {
                throw new ValidationException(
                    "Withdrawal currency must match the account currency.");
            }
            if (withdrawal.Amount > Balance.Amount)
            {
                throw new ValidationException(
                    "Insufficient funds for this withdrawal.");
            }
            
            Balance = Balance.Subtract(withdrawal);
        }
        //..................................................................
        public void PayCreditCard(Money payment)
        {
            if (AccountType != AccountType.CreditCard)
            {
                throw new ValidationException(
                    "Payments are only allowed for credit card accounts.");
            }
            ArgumentNullException.ThrowIfNull(payment);
            if(payment.Currency != Balance.Currency)
            {
                throw new ValidationException(
                    "Payment currency must match the account currency.");
            }
            if (payment.Amount <= 0)
            {
                throw new ValidationException(
                    "Payment amount must be greater than zero.");
            }
           
            if(payment.Amount > Balance.Amount)
            {
                throw new ValidationException(
                    "Payment amount exceeds the outstanding balance.");
            }
            Balance = Balance.Subtract(payment);
        }
        //..................................................................
        public void MakeCreditCardPurchase(Money purchase)
        {
            if (AccountType != AccountType.CreditCard)
            {
                throw new ValidationException(
                    "Purchases are only allowed for credit card accounts.");
            }
            ArgumentNullException.ThrowIfNull(purchase);
            if(purchase.Currency != Balance.Currency)
            {
                throw new ValidationException(
                    "Purchase currency must match the account currency.");
            }
            if (purchase.Amount <= 0)
            {
                throw new ValidationException(
                    "Purchase amount must be greater than zero.");
            }
           
            if (purchase.Amount+Balance.Amount>CreditLimit!.Amount)
            {
                throw new ValidationException(
                    "Purchase amount exceeds the credit limit.");
            }
            Balance =Balance.Add(purchase);
        }
    }
}
