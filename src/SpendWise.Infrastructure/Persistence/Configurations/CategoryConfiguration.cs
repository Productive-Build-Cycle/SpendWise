using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpendWise.Domain.Entities;

namespace SpendWise.Infrastructure.Persistence.Configurations
{
    internal class CategoryConfiguration: IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(category => category.Id);

            builder.HasOne(category => category.User)
                  .WithMany(user => user.Categories)
                  .HasForeignKey(category => category.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

            builder.Property(category => category.CategoryName)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(category => category.Description)
                   .HasMaxLength(500)
                   .IsRequired(false);
        }
    
    }
}
