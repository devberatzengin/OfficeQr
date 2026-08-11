using Microsoft.AspNetCore.Identity;

namespace OfficeQr.Entity;

public class User : IdentityUser<Guid>
{   

    public DateTime CreatedOn {get; set;} = DateTime.UtcNow;
    public DateTime UpdatedOn {get; set;} = DateTime.UtcNow;
    
    public bool IsDeleted {get; set;} = false;
    public bool IsActive {get; set;} = true;
}