using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OfficeQr.Entity;

namespace OfficeQr.Data.Configurations;

public class ItemStatusHistoryConfiguration : IEntityTypeConfiguration<ItemStatusHistory>
{
    public void Configure(EntityTypeBuilder<ItemStatusHistory> builder)
    {
        builder.ToTable("ItemStatusHistories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ChangedAt)
            .IsRequired();

        builder.HasOne(x => x.Item)
            .WithMany(i => i.StatusHistories)
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.ItemId, x.ChangedAt });
        builder.HasIndex(x => new { x.UserId, x.ChangedAt });
    }
}