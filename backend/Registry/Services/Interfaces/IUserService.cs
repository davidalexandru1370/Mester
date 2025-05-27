using System.Security.Claims;
using Registry.DTO;
using Registry.Models;

namespace Registry.Services.Interfaces;

public interface IUserService
{
    Task<User?> GetByClaims(ClaimsPrincipal claims);
    TokenResponse CreateToken(TokenRegistrationRequest request);
    Task<User> CreateUser(string username, string password, string phoneNumber, string email);
    Task<User?> LoginUser(string email, string password);
    Task<UserDetailsDto> GetUserDetailsById(Guid userId);
}