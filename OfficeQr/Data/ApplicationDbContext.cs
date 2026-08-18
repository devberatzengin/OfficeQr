using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OfficeQr.Data.Configurations;
using OfficeQr.Data.Interfaces;
using OfficeQr.Entity;

namespace OfficeQr.Data;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>, IApplicationDbContext
{

    public DbSet<Item> Items {get; set;}
    public DbSet<Shelf> Shelves {get; set;}
    public DbSet<Cabinet> Cabinets {get; set;}


    public DbSet<ItemShelfHistory> ItemShelfHistories { get; set; }
    public DbSet<ItemUserHistory> ItemUserHistories { get; set; }
    public DbSet<ShelfCabinetHistory> ShelfCabinetHistories { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new ItemConfiguration());
        builder.ApplyConfiguration(new ShelfConfiguration());
        builder.ApplyConfiguration(new CabinetConfiguration());

        builder.ApplyConfiguration(new ItemShelfHistoryConfiguration());
        builder.ApplyConfiguration(new ItemUserHistoryConfiguration());
        builder.ApplyConfiguration(new ShelfCabinetHistoryConfiguration());

        builder.HasDefaultSchema("identity");
    }

}
