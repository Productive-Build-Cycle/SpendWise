using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpendWise.Domain.Entities;

namespace SpendWise.Infrastructure.Persistence.Configurations
{
    internal class BudgetConfiguration: IEntityTypeConfiguration<Budget>
    {
        public void Configure(EntityTypeBuilder<Budget> builder)
        {
            builder.ToTable("Budgets");

            builder.HasKey(budget => budget.Id);

            builder.HasOne(budget => budget.User)
                   .WithMany(user => user.Budgets)
                   .HasForeignKey(budget => budget.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

          
            builder.Property(budget => budget.Amount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

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
