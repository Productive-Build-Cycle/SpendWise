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
            .HasColumnType("char(64)")
            .IsUnicode(false)
            .IsRequired();

        builder.Property(token => token.FamilyId)
            .IsRequired();

        builder.Property(token => token.CreatedAtUtc)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(token => token.ExpiresAtUtc)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(token => token.RevokedAtUtc)
            .HasColumnType("datetime2");

        builder.Property(token => token.RevocationReason)
            .HasConversion<byte>()
            .HasColumnType("tinyint");

        builder.Property(token => token.RowVersion)
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
