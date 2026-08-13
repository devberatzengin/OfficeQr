using System.ComponentModel.DataAnnotations;

namespace OfficeQr.Dtos.Item;


public class UpdateRequest
{
    [Required]
    public Guid Id {get; set;}

    public string? Name {get; set;} = string.Empty;

    public Guid? UserId {get; set;} = null;
    public Guid? ShelfId {get; set;} = null;
    public Guid? CabinetId {get; set;} = null;

}