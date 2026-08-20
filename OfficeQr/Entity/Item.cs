using OfficeQr.Entity.Enums;

namespace OfficeQr.Entity;

public class Item : BaseEntity
{

    public Guid Id {get; set;}
    public string QrCode {get; set;} = string.Empty;
    public string Name {get; set;} = string.Empty;

    public ItemStatus Status {get; set;} = ItemStatus.Available;


    public Guid? UserId {get; set;} = null;
    public User? User {get; set;} = null;

    public Guid? ShelfId {get; set;} = null;
    public Shelf? Shelf {get; set;} = null;

    public ICollection<ItemShelfHistory> ShelfHistories { get; set; } = new List<ItemShelfHistory>();
    public ICollection<ItemUserHistory> UserHistories { get; set; } = new List<ItemUserHistory>();
    public ICollection<ItemStatusHistory> StatusHistories { get; set; } = new List<ItemStatusHistory>();

    public uint Version { get; set; }
}