using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpendWise.Domain.Entities;

namespace SpendWise.Infrastructure.Persistence.Configurations
{
    internal class FinancialGoalConfiguration:IEntityTypeConfiguration<FinancialGoal>
    {
        public void Configure(EntityTypeBuilder<FinancialGoal> builder)
        {
            builder.ToTable("FinancialGoals");

            builder.HasKey(goal => goal.Id);

            builder.HasOne(goal => goal.User)
                   .WithMany(user => user.FinancialGoals)
                   .HasForeignKey(goal => goal.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(goal => goal.GoalName)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(goal => goal.TargetAmount)
                   .HasColumnType("decimal(18,2)")
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
