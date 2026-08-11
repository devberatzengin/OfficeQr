namespace OfficeQr.Entity;

public class Item : BaseEntity
{

    public Guid Id {get; set;}
    public string QrCode {get; set;} = string.Empty;
    public string Name {get; set;} = string.Empty;


    public Guid? UserId {get; set;} = null;
    public User? User {get; set;} = null;

    public Guid? ShelfId {get; set;} = null;
    public Shelf? Shelf {get; set;} = null;

    public Guid? CabinetId {get; set;} = null;
    public Cabinet? Cabinet{get; set;} = null;


}