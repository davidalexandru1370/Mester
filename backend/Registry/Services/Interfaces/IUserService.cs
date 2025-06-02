using Registry.DTO;
using Registry.Models;
using System.Security.Claims;

namespace Registry.Services.Interfaces;

public interface IUserService
{
    Task<User> GetByClaims(ClaimsPrincipal claims);
    Task<User> GetTradesManByClaims(ClaimsPrincipal claims);
    Task<User> GetClientByClaims(ClaimsPrincipal claims);
    TokenResponse CreateToken(TokenRegistrationRequest request);
    Task<User> CreateUser(string username, string password, string phoneNumber, string email);
    Task<User?> LoginUser(string email, string password);
    Task<UserDetailsDto> GetUserDetailsById(Guid userId);
    Task<UserImageDTO> UploadUserImage(IFormFile image, Guid userId);
}