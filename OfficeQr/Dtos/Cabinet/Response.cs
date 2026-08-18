namespace OfficeQr.Dtos.Cabinet;


public class Response
{
    public Guid Id {get; set;} 

    public short Capacity {get; set;}

    public IList<OfficeQr.Dtos.Shelf.Response> Shelves {get; set;} = new List<OfficeQr.Dtos.Shelf.Response>();
    
}