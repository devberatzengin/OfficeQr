using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OfficeQr.Entity;

namespace OfficeQr.Data.Configurations;

public class ShelfConfiguration : IEntityTypeConfiguration<Shelf>
{
    public void Configure(EntityTypeBuilder<Shelf> builder)
    {
        builder.ToTable("Shelves");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.QrCode)
            .IsRequired();

        builder.HasIndex(s => s.QrCode)
            .IsUnique();

        builder.HasOne(s => s.Cabinet)
            .WithMany(c => c.Shelves)
            .HasForeignKey(s => s.CabinetId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
