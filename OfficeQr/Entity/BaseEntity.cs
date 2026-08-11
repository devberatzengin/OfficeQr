using Microsoft.AspNetCore.Http.HttpResults;

namespace OfficeQr.Entity;

public class BaseEntity
{
    public DateTime CreatedOn {get; set;} = DateTime.UtcNow;
    public DateTime UpdatedOn {get; set;} = DateTime.UtcNow;
    
    public bool IsDeleted {get; set;} = false;
    public bool IsActive {get; set;} = true;
}