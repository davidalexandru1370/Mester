using Microsoft.EntityFrameworkCore;

namespace Registry.Models;

[Index(nameof(Email), IsUnique = true)]
public class User
{
    public Guid Id { get; set; }

    public required string Name { get; set; }
    
    public required string Email { get; set; }

    public required string HashPassword { get; set; }

    public required string Salt { get; set; }

    public required string PhoneNumber { get; set; }

    public TradesMan? TradesManProfile { get; set; }

    public string ImageUrl { get; set; }
}
