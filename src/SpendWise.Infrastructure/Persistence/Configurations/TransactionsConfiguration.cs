using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpendWise.Domain.Entities;

namespace SpendWise.Infrastructure.Persistence.Configurations
{
    internal class TransactionsConfiguration:IEntityTypeConfiguration<Transactions>
    {
        public void Configure(EntityTypeBuilder<Transactions> builder)
        {
            builder.ToTable("Transactions");
            builder.HasKey(transaction => transaction.Id);

            builder.HasOne(transaction => transaction.User)
                .WithMany(user => user.Transactions)
                .HasForeignKey(transaction => transaction.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(transaction => transaction.SourceAccount)
                    .WithMany()
                    .HasForeignKey(transaction => transaction.SourceAccountId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(transaction => transaction.DestinationAccount)
                    .WithMany()
                    .HasForeignKey(transaction => transaction.DestinationAccountId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.Property(transaction => transaction.Amount)
                   .HasColumnType("decimal(18,2)")
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
