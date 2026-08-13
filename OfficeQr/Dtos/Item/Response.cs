namespace OfficeQr.Dtos.Item;

public class Response
{
    public Guid Id {get; set;}
    public string QrCode {get; set;}
    public string Name {get; set;}

    public Guid? UserId {get; set;}
    public Guid? ShelfId {get; set;}
    public Guid? CabinetId {get; set;}

}