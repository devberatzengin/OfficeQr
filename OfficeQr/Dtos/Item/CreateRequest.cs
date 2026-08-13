
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OfficeQr.Dtos.Item;


public class CreateRequest
{
    [Required]
    public string Name {get; set;}
} 