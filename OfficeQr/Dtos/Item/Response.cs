namespace OfficeQr.Dtos.Item;

public class Response
{
    public Guid Id {get; set;}
    public string QrCode {get; set;} = string.Empty;
    public string Name {get; set;} = string.Empty;
    public string Status {get; set;} = string.Empty;

    public Guid? UserId {get; set;}
    public Guid? ShelfId {get; set;}
}