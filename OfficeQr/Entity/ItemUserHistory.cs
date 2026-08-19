using OfficeQr.Entity;

public class ItemUserHistory
{
    public Guid Id { get; set; }

    public Guid ItemId { get; set; }
    public Item Item { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public Guid? AssignedByUserId { get; set; }
    public User? AssignedByUser { get; set; }

    public DateTime? ReturnedAt { get; set; } = null;
    public Guid? ReturnedByUserId { get; set; }
    public User? ReturnedByUser { get; set; }
    public ItemMovementReason? ReturnedReason { get; set; }

    public ItemMovementReason Reason { get; set; }
}