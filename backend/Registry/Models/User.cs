namespace Registry.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public required string Username { get; set; }

    public required string HashPassword { get; set; }

    public required string Salt { get; set; }
}
