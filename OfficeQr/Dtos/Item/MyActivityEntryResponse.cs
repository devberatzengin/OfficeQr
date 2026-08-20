namespace OfficeQr.Dtos.Item;

public class MyActivityEntryResponse
{
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Shelf or User
    public string Phase { get; set; }  = string.Empty; // Opened or Closed
    public Guid? ShelfId { get; set; }
    public DateTime OccurredAt { get; set; }
}