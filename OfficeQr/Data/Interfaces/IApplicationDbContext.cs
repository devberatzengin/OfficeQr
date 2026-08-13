using OfficeQr.Entity;
using Microsoft.EntityFrameworkCore;

namespace OfficeQr.Data.Interfaces;
   
public interface IApplicationDbContext
{
    DbSet<Item> Items {get;}
    DbSet<Shelf> Shelves {get;}
    DbSet<Cabinet> Cabinets {get;}


    DbSet<T> Set<T>() where T : class;
    
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}