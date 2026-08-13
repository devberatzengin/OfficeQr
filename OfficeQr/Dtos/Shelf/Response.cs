using OfficeQr.Entity;

namespace OfficeQr.Dtos.Shelf;

public class Response
{
    public Guid Id {get; set;}

    public string QrCode {get; set;}

    public short Capacity {get; set;}

    public Guid CabimetId {get; set;}

}