using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpendWise.Domain.Entities;
using SpendWise.Domain.ValueObjects;

namespace SpendWise.Infrastructure.Persistence.Configurations;

public sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts", table =>
        {
            table.HasCheckConstraint(
                "CK_Accounts_OpeningBalance_NonNegative",
                "\"OpeningBalance_Amount\" >= 0");

            table.HasCheckConstraint(
                "CK_Accounts_Balance_NonNegative",
                "\"Balance_Amount\" >= 0");

            table.HasCheckConstraint(
                "CK_Accounts_CreditLimit_Positive",
                "\"CreditLimit_Amount\" IS NULL OR \"CreditLimit_Amount\" > 0");

            table.HasCheckConstraint(
                "CK_Accounts_Currency_USD",
                """
                "OpeningBalance_Currency" = 'USD'
                AND "Balance_Currency" = 'USD'
                AND
                (
                    ("CreditLimit_Amount" IS NULL AND "CreditLimit_Currency" IS NULL)
                    OR
                    ("CreditLimit_Amount" IS NOT NULL AND "CreditLimit_Currency" = 'USD')
                )
                """);

            table.HasCheckConstraint(
                "CK_Accounts_AccountType_Valid",
                "\"AccountType\" IN (1, 2, 3, 4, 5, 6, 7, 8)");

            table.HasCheckConstraint(
                "CK_Accounts_CreditLimit_AccountType",
                """
                (
                    "AccountType" = 4
                    AND "CreditLimit_Amount" IS NOT NULL
                    AND "CreditLimit_Currency" = 'USD'
                )
                OR
                (
                    "AccountType" <> 4
                    AND "CreditLimit_Amount" IS NULL
                    AND "CreditLimit_Currency" IS NULL
                )
                """);

            table.HasCheckConstraint(
                "CK_Accounts_CreditCardBalance_WithinLimit",
                """
                "AccountType" <> 4
                OR
                (
                    "CreditLimit_Amount" IS NOT NULL
                    AND "OpeningBalance_Amount" <= "CreditLimit_Amount"
                    AND "Balance_Amount" <= "CreditLimit_Amount"
                )
                """);

            table.HasCheckConstraint(
                "CK_Accounts_Name_NotBlank",
                "length(btrim(\"AccountName\")) > 0");
        });

        builder.HasKey(account => account.Id);

        builder.HasOne(account => account.User)
            .WithMany()
            .HasForeignKey(account => account.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(account => account.AccountName)
            .HasMaxLength(100)
            .IsRequired();

        builder.OwnsOne(account => account.OpeningBalance, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("OpeningBalance_Amount")
                .HasPrecision(18, 2)
                .IsRequired();

            money.Property(m => m.Currency)
                .HasConversion(
                    currency => currency.Code,
                    code => new Currency(code))
                .HasColumnName("OpeningBalance_Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(account => account.AccountType)
            .HasConversion<short>()
            .HasColumnType("smallint")
            .IsRequired();

        builder.OwnsOne(account => account.CreditLimit, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("CreditLimit_Amount")
                .HasPrecision(18, 2)
                .IsRequired();

            money.Property(m => m.Currency)
                .HasConversion(
                    currency => currency.Code,
                    code => new Currency(code))
                .HasColumnName("CreditLimit_Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Navigation(account => account.CreditLimit)
            .IsRequired(false);

        builder.OwnsOne(account => account.Balance, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Balance_Amount")
                .HasPrecision(18, 2)
                .IsRequired();

            money.Property(m => m.Currency)
                .HasConversion(
                    currency => currency.Code,
                    code => new Currency(code))
                .HasColumnName("Balance_Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.HasMany(account => account.SourceTransactions)
            .WithOne(transaction => transaction.SourceAccount)
            .HasForeignKey(transaction => transaction.SourceAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(account => account.DestinationTransactions)
            .WithOne(transaction => transaction.DestinationAccount)
            .HasForeignKey(transaction => transaction.DestinationAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(account => account.SourceTransactions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(account => account.DestinationTransactions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}