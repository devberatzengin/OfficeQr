using System.ComponentModel.DataAnnotations;

namespace OfficeQr.Dtos.Cabinet;

public class UpdateRequest
{
    
    [Range(1,32767)]
    public short? Capacity {get; set;}
    
}