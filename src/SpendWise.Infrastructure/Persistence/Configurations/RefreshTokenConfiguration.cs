using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpendWise.Infrastructure.Authentication.RefreshTokens;
using SpendWise.Infrastructure.Identity;

namespace SpendWise.Infrastructure.Persistence.Configurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshTokenEntity>
{
    public void Configure(EntityTypeBuilder<RefreshTokenEntity> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(token => token.Id);

        builder.Property(token => token.TokenHash)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(token => token.FamilyId)
            .IsRequired();

        builder.Property(token => token.CreatedAtUtc)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(token => token.ExpiresAtUtc)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(token => token.RevokedAtUtc)
            .HasColumnType("timestamp with time zone");

        builder.Property(token => token.RevocationReason)
            .HasConversion<short>()
            .HasColumnType("smallint");

        builder.Property(token => token.Version)
            .IsRowVersion();

        builder.HasIndex(token => token.TokenHash)
            .IsUnique()
            .HasDatabaseName("UX_RefreshTokens_TokenHash");

        builder.HasIndex(token => new
            {
                token.FamilyId,
                token.ExpiresAtUtc
            })
            .HasDatabaseName("IX_RefreshTokens_FamilyId_ExpiresAtUtc");

        builder.HasIndex(token => new
            {
                token.UserId,
                token.ExpiresAtUtc
            })
            .HasDatabaseName("IX_RefreshTokens_UserId_ExpiresAtUtc");

        builder.HasIndex(token => token.ExpiresAtUtc)
            .HasDatabaseName("IX_RefreshTokens_ExpiresAtUtc");

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(token => token.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_RefreshTokens_AspNetUsers_UserId");

        builder.HasOne<RefreshTokenEntity>()
            .WithMany()
            .HasForeignKey(token => token.ReplacedByTokenId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_RefreshTokens_RefreshTokens_ReplacedByTokenId");
    }
}