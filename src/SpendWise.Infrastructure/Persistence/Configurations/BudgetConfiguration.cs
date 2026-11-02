using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpendWise.Domain.Entities;
using SpendWise.Domain.ValueObjects;

namespace SpendWise.Infrastructure.Persistence.Configurations
{
    internal class BudgetConfiguration: IEntityTypeConfiguration<Budget>
    {
        public void Configure(EntityTypeBuilder<Budget> builder)
        {
            builder.ToTable("Budgets", table =>
            {
                table.HasCheckConstraint(
                    "CK_Budgets_Amount_Positive",
                    "[Amount_Amount] > 0");

                table.HasCheckConstraint(
                    "CK_Budgets_DateRange_Valid",
                    "[StartDate] < [EndDate]");
            });

            builder.HasKey(budget => budget.Id);

            builder.HasOne(budget => budget.User)
                   .WithMany()
                   .HasForeignKey(budget => budget.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

          
            builder.OwnsOne(budget => budget.Amount, money =>
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

            builder.Property(budget => budget.StartDate)
                   .HasColumnType("date")
                   .IsRequired();

            builder.Property(budget => budget.EndDate)
                   .HasColumnType("date")
                   .IsRequired();

            builder.HasOne(budget => budget.Category)
                   .WithMany()
                   .HasForeignKey(budget => budget.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

        }
    
    }
}
