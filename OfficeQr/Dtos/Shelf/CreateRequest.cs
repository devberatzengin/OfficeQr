using System.ComponentModel.DataAnnotations;

namespace OfficeQr.Dtos.Shelf;


public class CreateRequest
{
    [Required]
    public short Capacity {get; set;} = 1;

    [Required]
    public Guid CabinetId {get; set;}
}