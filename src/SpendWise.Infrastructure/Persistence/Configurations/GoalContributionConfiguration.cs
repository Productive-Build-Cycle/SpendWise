using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpendWise.Domain.Entities;

namespace SpendWise.Infrastructure.Persistence.Configurations
{
    internal class GoalContributionConfiguration:IEntityTypeConfiguration<GoalContribution>
    {
        public void Configure(EntityTypeBuilder<GoalContribution> builder)
        {
            builder.ToTable("GoalContributions");
            builder.HasKey(gc => gc.Id);
            builder.HasOne(gc => gc.FinancialGoal)
                   .WithMany(goal=>goal.Contributions)
                   .HasForeignKey(gc => gc.FinancialGoalId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(gc => gc.ContributionAmount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(gc => gc.ContributionDate)
                   .HasColumnType("date")
                   .IsRequired();

            builder.HasOne(gc => gc.Account)
                    .WithMany()
                   .HasForeignKey(gc => gc.AccountId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(gc => gc.Transaction)
                   .WithMany()
                   .HasForeignKey(gc => gc.TransactionId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    
    }
}
