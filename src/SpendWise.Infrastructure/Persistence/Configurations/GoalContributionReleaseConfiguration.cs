using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpendWise.Domain.Entities;
using SpendWise.Domain.ValueObjects;

namespace SpendWise.Infrastructure.Persistence.Configurations
{
    internal class GoalContributionReleaseConfiguration : IEntityTypeConfiguration<GoalContributionRelease>
    {
        public void Configure(EntityTypeBuilder<GoalContributionRelease> builder)
        {
            builder.ToTable("GoalContributionReleases", table =>
            {
                table.HasCheckConstraint(
                    "CK_GoalContributionReleases_ReleaseAmount_Positive",
                    "[ReleaseAmount_Amount] > 0");
                table.HasCheckConstraint(
                    "CK_GoalContributionReleases_Currency_USD",
                    "[ReleaseAmount_Currency] = 'USD'");
            });
            builder.HasKey(release => release.Id);
            builder.HasOne(release => release.GoalContribution)
                   .WithMany(contribution => contribution.Releases)
                   .HasForeignKey(release => release.GoalContributionId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsOne(release => release.ReleaseAmount, money =>
            {
                money.Property(m => m.Amount)
                     .HasColumnName("ReleaseAmount_Amount")
                     .HasPrecision(18, 2)
                     .IsRequired();
                money.Property(m => m.Currency)
                        .HasConversion(
                            currency => currency.Code,
                            code => new Currency(code))
                        .HasColumnName("ReleaseAmount_Currency")
                        .HasMaxLength(3)
                        .IsRequired();
            });
                 
            builder.Property(release => release.ReleaseDate)
                   .HasColumnType("date")
                   .IsRequired();

            builder.HasOne(release => release.Transaction)
                   .WithMany()
                   .HasForeignKey(release => release.TransactionId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
