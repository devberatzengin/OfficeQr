using System.ComponentModel.DataAnnotations;

namespace OfficeQr.Dtos.Shelf;


public class CreateRequest
{
    [Required]
    [Range(1,32767)]
    public short Capacity {get; set;} = 1;

    [Required]
    public Guid CabinetId {get; set;}
}