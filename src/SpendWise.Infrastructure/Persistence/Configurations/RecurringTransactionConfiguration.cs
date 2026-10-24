using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpendWise.Domain.Entities;

namespace SpendWise.Infrastructure.Persistence.Configurations
{
    internal class RecurringTransactionConfiguration:IEntityTypeConfiguration<RecurringTransaction>
    {
        public void Configure(EntityTypeBuilder<RecurringTransaction> builder)
        {
            builder.ToTable("RecurringTransactions");

            builder.HasKey(rt => rt.Id);

            builder.HasOne(rt => rt.User)
                   .WithMany(user => user.RecurringTransactions)
                   .HasForeignKey(rt => rt.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(rt => rt.TransactionName)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(rt => rt.Amount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(rt => rt.Frequency)
                  .IsRequired();

            builder.Property(rt => rt.TransactionType)
                   .IsRequired();

            builder.Property(rt => rt.StartDate)
                   .HasColumnType("datetime2")
                   .IsRequired();

            builder.Property(rt => rt.NextOccurrenceDate)
                   .HasColumnType("datetime2")
                   .IsRequired();

            builder.Property(rt => rt.EndDate)
                   .HasColumnType("datetime2")
                   .IsRequired(false);

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
        }
    }
    
    
}
