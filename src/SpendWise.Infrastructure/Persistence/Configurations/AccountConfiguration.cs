using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SpendWise.Domain.Entities;
using SpendWise.Domain.ValueObjects;

namespace SpendWise.Infrastructure.Persistence.Configurations
{
    public class AccountConfiguration:IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Accounts", table =>
            {
                table.HasCheckConstraint(
                    "CK_Accounts_OpeningBalance_NonNegative",
                    "[OpeningBalance] >= 0");

                table.HasCheckConstraint(
                    "CK_Accounts_Balance_NonNegative",
                    "[Balance] >= 0");

                table.HasCheckConstraint(
                    "CK_Accounts_CreditLimit_Positive",
                    "[CreditLimit] IS NULL OR [CreditLimit] > 0");
                table.HasCheckConstraint(
                   "CK_Accounts_Currency_USD",
                   "[Currency] = 'USD'");
            });

            builder.HasKey(account => account.Id);
           
            builder.HasOne(account => account.User)
                .WithMany()
                .HasForeignKey(account => account.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(account => account.AccountName)
                   .HasMaxLength(100)
                   .IsRequired();
            var convertor = new ValueConverter<Currency, string>(
                 Currency => Currency.Code,
                    code => new Currency(code));
            builder.Property(account => account.Currency)
                    .HasConversion(convertor)
                   .HasMaxLength(3)
                   .IsRequired();
            builder.Property(account => account.OpeningBalance)
                    .HasPrecision(18, 2);

            builder.Property(account => account.Balance)
                    .HasPrecision(18, 2);

            builder.Property(account => account.AccountType)
                     .IsRequired();
            builder.Property(account => account.CreditLimit)
                   .HasPrecision(18,2)
                   .IsRequired(false);
            builder.HasMany(account=>account.SourceTransactions)
                    .WithOne(transaction=> transaction.SourceAccount)
                    .HasForeignKey(transaction => transaction.SourceAccountId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(account => account.DestinationTransactions)
                   .WithOne(transaction => transaction.DestinationAccount)
                    .HasForeignKey(transaction => transaction.DestinationAccountId)
                    .OnDelete(DeleteBehavior.Restrict);
        }
    
    }
}
