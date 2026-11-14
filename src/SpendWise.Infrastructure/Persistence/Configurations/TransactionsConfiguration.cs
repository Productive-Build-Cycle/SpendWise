using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpendWise.Domain.Entities;
using SpendWise.Domain.ValueObjects;

namespace SpendWise.Infrastructure.Persistence.Configurations;

public sealed class TransactionsConfiguration : IEntityTypeConfiguration<Transactions>
{
    public void Configure(EntityTypeBuilder<Transactions> builder)
    {
        builder.ToTable("Transactions", table =>
        {
            table.HasCheckConstraint(
                "CK_Transactions_Amount_Positive",
                "\"Amount_Amount\" > 0");

            table.HasCheckConstraint(
                "CK_Transactions_DifferentAccounts",
                """
                "SourceAccountId" IS NULL
                OR "DestinationAccountId" IS NULL
                OR "SourceAccountId" <> "DestinationAccountId"
                """);

            table.HasCheckConstraint(
                "CK_Transactions_AccountCombination",
                """
                ("TransactionType" = 1
                    AND "SourceAccountId" IS NULL
                    AND "DestinationAccountId" IS NOT NULL)
                OR
                ("TransactionType" = 2
                    AND "SourceAccountId" IS NOT NULL
                    AND "DestinationAccountId" IS NULL)
                OR
                ("TransactionType" = 3
                    AND "SourceAccountId" IS NOT NULL
                    AND "DestinationAccountId" IS NOT NULL)
                """);

            table.HasCheckConstraint(
                "CK_Transactions_CategoryCombination",
                """
                ("TransactionType" IN (1, 2) AND "CategoryId" IS NOT NULL)
                OR
                ("TransactionType" = 3 AND "CategoryId" IS NULL)
                """);
        });

        builder.HasKey(transaction => transaction.Id);

        builder.OwnsOne<Money>(transaction => transaction.Amount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Amount_Amount")
                .HasPrecision(18, 2)
                .IsRequired();

            money.Property(m => m.Currency)
                .HasConversion(
                    currency => currency.Code,
                    code => new Currency(code))
                .HasColumnName("Amount_Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(transaction => transaction.Description)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(transaction => transaction.TransactionType)
            .HasConversion<short>()
            .HasColumnType("smallint")
            .IsRequired();

        builder.Property(transaction => transaction.TransactionDate)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(transaction => transaction.CategoryId)
            .IsRequired(false);

        builder.HasOne(transaction => transaction.User)
            .WithMany()
            .HasForeignKey(transaction => transaction.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(transaction => transaction.SourceAccount)
            .WithMany(account => account.SourceTransactions)
            .HasForeignKey(transaction => transaction.SourceAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(transaction => transaction.DestinationAccount)
            .WithMany(account => account.DestinationTransactions)
            .HasForeignKey(transaction => transaction.DestinationAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(transaction => transaction.Category)
            .WithMany()
            .HasForeignKey(transaction => new
            {
                transaction.UserId,
                transaction.CategoryId
            })
            .HasPrincipalKey(category => new
            {
                category.UserId,
                category.Id
            })
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Transactions_Categories_UserId_CategoryId");
    }
}