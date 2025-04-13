namespace Registry.Models;

public record class TokenResponse(string jwt, int expiresIn);
