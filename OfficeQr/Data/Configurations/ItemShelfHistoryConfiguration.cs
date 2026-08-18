using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OfficeQr.Entity;

namespace OfficeQr.Data.Configurations;

public class ItemShelfHistoryConfiguration : IEntityTypeConfiguration<ItemShelfHistory>
{
    public void Configure(EntityTypeBuilder<ItemShelfHistory> builder)
    {
        builder.ToTable("ItemShelfHistories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PlacedAt)
            .IsRequired();

        builder.HasOne(x => x.Item)
            .WithMany(i => i.ShelfHistories)
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);
    
        builder.HasOne(x => x.Shelf)
            .WithMany() 
            .HasForeignKey(x => x.ShelfId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.ItemId, x.RemovedAt });
    }
}