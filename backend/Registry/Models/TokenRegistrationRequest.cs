using System.Text.Json;
namespace Registry.Models;


public class TokenRegistrationRequest
{
    public required Guid Id { get; set; }
    public required string Email { get; set; }
    public required Dictionary<string, JsonElement> CustomClaims { get; set; }
}
