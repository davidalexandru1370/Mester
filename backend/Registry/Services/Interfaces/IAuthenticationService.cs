using System.Security.Claims;
using Registry.Models;

namespace Registry.Services.Interfaces;

public interface IAuthenticationService
{
    Task<User?> GetByClaims(ClaimsPrincipal claims);
    TokenResponse CreateToken(TokenRegistrationRequest request);
    Task<User> CreateUser(string username, string password, string phoneNumber);
    Task<User?> LoginUser(string username, string password);
}