using OfficeQr.Entity.Enums;

namespace OfficeQr.Entity;

public class ItemStatusHistory
{
    public Guid Id { get; set; }

    public Guid ItemId { get; set; }
    public Item Item { get; set; } = null!;

    public ItemStatus Status { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}