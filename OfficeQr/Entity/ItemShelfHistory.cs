namespace OfficeQr.Entity;


public class ItemShelfHistory
{
    public Guid Id { get; set; }
 
    public Guid ItemId { get; set; }
    public Item Item { get; set; } = null!;

    public Guid ShelfId { get; set; }
    public Shelf Shelf { get; set; } = null!;
 
    public DateTime PlacedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RemovedAt { get; set; } = null;
}
