namespace OfficeQr.Entity;

public class ShelfCabinetHistory
{
    public Guid Id { get; set; }
 
    public Guid ShelfId { get; set; }
    public Shelf Shelf { get; set; } = null!;
 
    public Guid CabinetId { get; set; }
    public Cabinet Cabinet { get; set; } = null!;
 
    public DateTime MovedInAt { get; set; } = DateTime.UtcNow;
    public DateTime? MovedOutAt { get; set; } = null;
}