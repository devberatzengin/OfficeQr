namespace OfficeQr.Entity;

public class Shelf : BaseEntity
{
    public Guid Id {get; set;}
    
    public string QrCode {get; set;} = string.Empty;

    public short Capacity {get; set;} = 1;

    public Guid CabinetId {get; set;} 
    public Cabinet Cabinet {get; set;} 

    public ICollection<Item> Items {get; set;} = new List<Item>();
}