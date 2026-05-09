using Microsoft.AspNetCore.Identity;

namespace Rupestre.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string? NomeCompleto { get; set; }
}
