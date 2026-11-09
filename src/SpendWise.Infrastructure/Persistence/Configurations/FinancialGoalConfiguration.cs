using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpendWise.Domain.Entities;
using SpendWise.Domain.ValueObjects;

namespace SpendWise.Infrastructure.Persistence.Configurations;

internal class FinancialGoalConfiguration : IEntityTypeConfiguration<FinancialGoal>
{
    public void Configure(EntityTypeBuilder<FinancialGoal> builder)
    {
        builder.ToTable("FinancialGoals", table =>
        {
            table.HasCheckConstraint(
                "CK_FinancialGoals_TargetAmount_Positive",
                "\"TargetAmount_Amount\" > 0");

            table.HasCheckConstraint(
                "CK_FinancialGoals_Currency_USD",
                "\"TargetAmount_Currency\" = 'USD'");
        });

        builder.HasKey(goal => goal.Id);

        builder.HasOne(goal => goal.User)
            .WithMany()
            .HasForeignKey(goal => goal.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(goal => goal.GoalName)
            .HasMaxLength(100)
            .IsRequired();

        builder.OwnsOne(goal => goal.TargetAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("TargetAmount_Amount")
                .HasPrecision(18, 2)
                .IsRequired();

            money.Property(m => m.Currency)
                .HasConversion(
                    currency => currency.Code,
                    code => new Currency(code))
                .HasColumnName("TargetAmount_Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

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