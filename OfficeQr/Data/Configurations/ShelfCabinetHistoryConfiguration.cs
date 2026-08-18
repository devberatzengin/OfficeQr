using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OfficeQr.Entity;

namespace OfficeQr.Data.Configurations;

public class ShelfCabinetHistoryConfiguration : IEntityTypeConfiguration<ShelfCabinetHistory>
{
    public void Configure(EntityTypeBuilder<ShelfCabinetHistory> builder)
    {
        builder.ToTable("ShelfCabinetHistories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.MovedInAt)
            .IsRequired();

        builder.HasOne(x => x.Shelf)
            .WithMany() 
            .HasForeignKey(x => x.ShelfId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Cabinet)
            .WithMany() 
            .HasForeignKey(x => x.CabinetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.ShelfId, x.MovedOutAt });
    }
}