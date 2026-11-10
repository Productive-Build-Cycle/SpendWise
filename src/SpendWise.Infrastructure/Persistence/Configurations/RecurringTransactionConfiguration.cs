
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpendWise.Domain.Entities;
using SpendWise.Domain.ValueObjects;

namespace SpendWise.Infrastructure.Persistence.Configurations;

internal sealed class RecurringTransactionConfiguration
    : IEntityTypeConfiguration<RecurringTransaction>
{
    public void Configure(EntityTypeBuilder<RecurringTransaction> builder)
    {
        builder.ToTable("RecurringTransactions", table =>
        {
            table.HasCheckConstraint(
                "CK_RecurringTransactions_Amount_Positive",
                "\"Amount_Amount\" > 0");

            table.HasCheckConstraint(
                "CK_RecurringTransactions_DifferentAccounts",
                """
                "SourceAccountId" IS NULL
                OR "DestinationAccountId" IS NULL
                OR "SourceAccountId" <> "DestinationAccountId"
                """);

            table.HasCheckConstraint(
                "CK_RecurringTransactions_ValidDates",
                """
                "StartDate" <= "NextOccurrenceDate"
                AND
                (
                    "EndDate" IS NULL
                    OR
                    (
                        "EndDate" >= "StartDate"
                        AND "NextOccurrenceDate" <= "EndDate"
                    )
                )
                """);

            table.HasCheckConstraint(
                "CK_RecurringTransactions_AccountCombination",
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
        });

        builder.HasKey(rt => rt.Id);

        builder.HasOne(rt => rt.User)
            .WithMany()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(rt => rt.TransactionName)
            .HasMaxLength(100)
            .IsRequired();

        builder.OwnsOne(rt => rt.Amount, money =>
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

        builder.Property(rt => rt.Frequency)
            .HasConversion<short>()
            .HasColumnType("smallint")
            .IsRequired();

        builder.Property(rt => rt.TransactionType)
            .HasConversion<short>()
            .HasColumnType("smallint")
            .IsRequired();

        builder.HasOne(rt => rt.SourceAccount)
            .WithMany()
            .HasForeignKey(rt => rt.SourceAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(rt => rt.DestinationAccount)
            .WithMany()
            .HasForeignKey(rt => rt.DestinationAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(rt => rt.Category)
            .WithMany()
            .HasForeignKey(rt => rt.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(rt => rt.CategoryId)
            .IsRequired(false);

        builder.Property(rt => rt.StartDate)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(rt => rt.NextOccurrenceDate)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(rt => rt.EndDate)
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);
    }
}
