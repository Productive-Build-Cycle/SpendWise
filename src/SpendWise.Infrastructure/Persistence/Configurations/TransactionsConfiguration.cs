using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpendWise.Domain.Entities;

namespace SpendWise.Infrastructure.Persistence.Configurations
{
    public class TransactionsConfiguration:IEntityTypeConfiguration<Transactions>
    {
        public void Configure(EntityTypeBuilder<Transactions> builder)
        {
            builder.ToTable("Transactions", table =>
            {
                builder.ToTable("Transactions", table =>
                {
                    table.HasCheckConstraint(
                        "CK_Transactions_Amount_Positive",
                        "[Amount] > 0");

                    table.HasCheckConstraint(
                        "CK_Transactions_DifferentAccounts",
                        "[SourceAccountId] IS NULL OR [DestinationAccountId] IS NULL " +
                        "OR [SourceAccountId] <> [DestinationAccountId]");

                    table.HasCheckConstraint(
                        "CK_Transactions_AccountCombination",
                        "([TransactionType] = 1 AND [SourceAccountId] IS NULL AND [DestinationAccountId] IS NOT NULL) OR " +
                        "([TransactionType] = 2 AND [SourceAccountId] IS NOT NULL AND [DestinationAccountId] IS NULL) OR " +
                        "([TransactionType] = 3 AND [SourceAccountId] IS NOT NULL AND [DestinationAccountId] IS NOT NULL)");
                });
            });
            builder.HasKey(transaction => transaction.Id);

            builder.HasOne(transaction => transaction.User)
                .WithMany()
                .HasForeignKey(transaction => transaction.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(transaction => transaction.SourceAccount)
                    .WithMany(account=>account.SourceTransactions)
                    .HasForeignKey(transaction => transaction.SourceAccountId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(transaction => transaction.DestinationAccount)
                    .WithMany(account=>account.DestinationTransactions)
                    .HasForeignKey(transaction => transaction.DestinationAccountId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.Property(transaction => transaction.Amount)
                   .HasPrecision(18, 2)
                      .IsRequired();
            builder.Property(transaction => transaction.Description)
                     .HasMaxLength(500)
                     .IsRequired(false);
            builder.Property(transaction=>transaction.TransactionType)
                 .IsRequired();

            builder.Property(transaction => transaction.TransactionDate)
                    .HasColumnType("datetime2")
                   .IsRequired();

        }
    }
}
