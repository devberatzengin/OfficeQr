using System.ComponentModel.DataAnnotations;

namespace OfficeQr.Dtos.Item;


public class ReturnRequest
{
    //[Required]
    //public Guid UserId {get; set;} =  Guid.Empty;

    [Required]
    public Guid ItemId {get; set;} =  Guid.Empty;
    
    [Required]  
    public Guid ShelfId {get; set;} =  Guid.Empty;
    
}