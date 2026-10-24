using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpendWise.Domain.Entities;

namespace SpendWise.Infrastructure.Persistence.Configurations
{
    public class AccountConfiguration:IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Accounts");

            builder.HasKey(account => account.Id);
           
            builder.HasOne(account => account.User)
                .WithMany(user => user.Accounts)
                .HasForeignKey(account => account.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(account => account.AccountName)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(account => account.Currency)
                   .HasMaxLength(3)
                   .IsRequired();
            builder.Property(account => account.OpeningBalance)
                   .HasColumnType("decimal(18,2)");

            builder.Property(account => account.Balance)
                   .HasColumnType("decimal(18,2)");

            builder.Property(account => account.AccountType)
                     .IsRequired();
        }
    
    }
}
