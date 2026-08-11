namespace OfficeQr.Entity;

public class Cabinet : BaseEntity
{
    public Guid Id {get; set;}
    public string QrCode {get; set;} = string.Empty;

    public short Capacity {get; set;} = 3;    

    public ICollection<Shelf> Shelves {get; set;} = new List<Shelf>();
}