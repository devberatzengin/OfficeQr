using OfficeQr.Entity;
using OfficeQr.Entity.Enums;

public class ItemShelfHistory
{
    public Guid Id { get; set; }

    public Guid ItemId { get; set; }
    public Item Item { get; set; } = null!;

    public Guid ShelfId { get; set; }
    public Shelf Shelf { get; set; } = null!;

    public DateTime PlacedAt { get; set; } = DateTime.UtcNow;
    public Guid? PlacedByUserId { get; set; }
    public User? PlacedByUser { get; set; }

    public DateTime? RemovedAt { get; set; } = null;
    public Guid? RemovedByUserId { get; set; }
    public User? RemovedByUser { get; set; }
    public ItemMovementReason? RemovedReason { get; set; }

    public ItemMovementReason Reason { get; set; }
}