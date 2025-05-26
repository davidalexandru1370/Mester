using System.Security.Claims;
using Registry.DTO;
using Registry.Models;

namespace Registry.Services.Interfaces;

public interface IUserService
{
    Task<User?> GetByClaims(ClaimsPrincipal claims);
    TokenResponse CreateToken(TokenRegistrationRequest request);
    Task<User> CreateUser(string username, string password, string phoneNumber);
    Task<User?> LoginUser(string username, string password);
    Task<UserDetailsDto> GetUserDetailsById(Guid userId);
}