using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpendWise.Domain.Entities;

namespace SpendWise.Infrastructure.Persistence.Configurations
{
    internal class FinancialGoalConfiguration:IEntityTypeConfiguration<FinancialGoal>
    {
        public void Configure(EntityTypeBuilder<FinancialGoal> builder)
        {
            builder.ToTable("FinancialGoals", table =>
            {
                table.HasCheckConstraint(
                    "CK_FinancialGoals_TargetAmount_Positive",
                    "[TargetAmount] > 0");
            });

            builder.HasKey(goal => goal.Id);

            builder.HasOne(goal => goal.User)
                   .WithMany()
                   .HasForeignKey(goal => goal.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(goal => goal.GoalName)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(goal => goal.TargetAmount)
                   .HasPrecision(18, 2)
                   .IsRequired();

           
            builder.Property(goal => goal.TargetDate)
                   .HasColumnType("date")
                   .IsRequired();

            builder.HasMany(goal => goal.Contributions)
                     .WithOne(contribution => contribution.FinancialGoal)
                     .HasForeignKey(contribution => contribution.FinancialGoalId)
                     .OnDelete(DeleteBehavior.Restrict);

            builder.Property(goal => goal.IsArchived)
                   .HasDefaultValue(false)
                   .IsRequired();
        }
    
    }
}
