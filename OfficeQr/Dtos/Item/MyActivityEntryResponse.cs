namespace OfficeQr.Dtos.Item;

public class MyActivityEntryResponse
{
    public Guid ItemId { get; set; }
    public string ItemName { get; set; }
    public string Action { get; set; }
    public string Type { get; set; }      // "Shelf" ya da "User"
    public string Phase { get; set; }     // "Opened" ya da "Closed"
    public Guid? ShelfId { get; set; }
    public DateTime OccurredAt { get; set; }
}