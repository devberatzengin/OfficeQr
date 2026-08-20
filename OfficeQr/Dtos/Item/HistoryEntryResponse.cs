namespace OfficeQr.Dtos.Item;

public class HistoryEntryResponse
{
    public string Action { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Shelf or User olabilir
    public string Phase { get; set; } = string.Empty; // Opened Closed olabilir
    public Guid? ShelfId { get; set; }
    public Guid? UserId { get; set; } // konu: item kimde (User tipi satırlarda)
    public Guid? ActorUserId { get; set; } // bu olayı kim yaptı
    public DateTime OccurredAt { get; set; }
}