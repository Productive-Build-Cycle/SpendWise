using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpendWise.Domain.Entities;
using SpendWise.Domain.ValueObjects;

namespace SpendWise.Infrastructure.Persistence.Configurations
{
    internal class GoalContributionConfiguration:IEntityTypeConfiguration<GoalContribution>
    {
        public void Configure(EntityTypeBuilder<GoalContribution> builder)
        {
            builder.ToTable("GoalContributions", table =>
            {
                table.HasCheckConstraint(
                    "CK_GoalContributions_ContributionAmount_Positive",
                    "[ContributionAmount_Amount] > 0");
                table.HasCheckConstraint(
                    "CK_GoalContributions_Currency_USD",
                    "[ContributionAmount_Currency] = 'USD'");
            });

            builder.HasKey(gc => gc.Id);
            builder.HasOne(gc => gc.FinancialGoal)
                   .WithMany(goal=>goal.Contributions)
                   .HasForeignKey(gc => gc.FinancialGoalId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsOne(gc => gc.ContributionAmount, money =>
            {
                money.Property(m => m.Amount)
                     .HasColumnName("ContributionAmount_Amount")
                     .HasPrecision(18, 2)
                     .IsRequired();
                money.Property(m => m.Currency)
                     .HasConversion(
                        currency => currency.Code,
                        code => new Currency(code))
                     .HasColumnName("ContributionAmount_Currency")
                     .HasMaxLength(3)
                     .IsRequired();
            });

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
