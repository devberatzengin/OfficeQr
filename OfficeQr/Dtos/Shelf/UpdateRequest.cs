using System.ComponentModel.DataAnnotations;

namespace OfficeQr.Dtos.Shelf;


public class UpdateRequest
{
    [Required]
    public Guid Id {get; set;}
    
    public short? Capacity {get; set;}

}