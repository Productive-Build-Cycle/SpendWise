using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpendWise.Domain.Entities;

namespace SpendWise.Infrastructure.Persistence.Configurations
{
    internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories", table =>
            {
                table.HasCheckConstraint(
                    "CK_Categories_CategoryType_Valid",
                    "\"CategoryType\" IN (1, 2)");
            });

            builder.HasKey(category => category.Id);

            builder.HasAlternateKey(category => new
                {
                    category.UserId,
                    category.Id
                })
                .HasName("AK_Categories_UserId_Id");

            builder.Property(category => category.CategoryName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(category => category.Description)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(category => category.CategoryType)
                .HasConversion<short>()
                .HasColumnType("smallint")
                .IsRequired();

            builder.Property(category => category.IsDefault)
                .HasDefaultValue(false)
                .IsRequired();

            builder.HasOne(category => category.User)
                .WithMany()
                .HasForeignKey(category => category.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}