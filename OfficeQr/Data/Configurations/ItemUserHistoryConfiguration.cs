using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OfficeQr.Entity;

namespace OfficeQr.Data.Configurations;

public class ItemUserHistoryConfiguration : IEntityTypeConfiguration<ItemUserHistory>
{
    public void Configure(EntityTypeBuilder<ItemUserHistory> builder)
    {
        builder.ToTable("ItemUserHistories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AssignedAt)
            .IsRequired();

        builder.HasOne(x => x.Item)
            .WithMany(i => i.UserHistories)
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany() 
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AssignedByUser)
            .WithMany()
            .HasForeignKey(x => x.AssignedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ReturnedByUser)
            .WithMany()
            .HasForeignKey(x => x.ReturnedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.ItemId, x.ReturnedAt });
    }
}