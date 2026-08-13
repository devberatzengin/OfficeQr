using System.ComponentModel.DataAnnotations;

namespace OfficeQr.Dtos.Shelf;


public class UpdateRequest
{
    [Required]
    public Guid Id {get; set;}
    

    [Range(1,32767)]
    public short? Capacity {get; set;}

}