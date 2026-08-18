namespace OfficeQr.Dtos.Item;

public class MoveRequest
{
    public Guid ShelfId {get; set;}
    public Guid? UserId {get; set;} = null;
}
