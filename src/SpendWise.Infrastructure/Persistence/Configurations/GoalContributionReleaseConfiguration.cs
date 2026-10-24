using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpendWise.Domain.Entities;

namespace SpendWise.Infrastructure.Persistence.Configurations
{
    internal class GoalContributionReleaseConfiguration : IEntityTypeConfiguration<GoalContributionRelease>
    {
        public void Configure(EntityTypeBuilder<GoalContributionRelease> builder)
        {
            builder.ToTable("GoalContributionReleases");
            builder.HasKey(release => release.Id);
            builder.HasOne(release => release.GoalContribution)
                   .WithMany(contribution => contribution.Releases)
                   .HasForeignKey(release => release.GoalContributionId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.Property(release => release.ReleaseAmount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(release => release.ReleaseDate)
                   .HasColumnType("datetime2")
                   .IsRequired();

            builder.HasOne(release => release.Transaction)
                   .WithMany()
                   .HasForeignKey(release => release.TransactionId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
