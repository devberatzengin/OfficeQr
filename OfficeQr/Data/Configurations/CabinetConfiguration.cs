using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OfficeQr.Entity;

namespace OfficeQr.Data.Configurations;

public class CabinetConfiguration : IEntityTypeConfiguration<Cabinet>
{
    public void Configure(EntityTypeBuilder<Cabinet> builder)
    {
        builder.ToTable("Cabinets");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.QrCode)
            .IsRequired();

        builder.HasIndex(c => c.QrCode)
            .IsUnique();

        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
