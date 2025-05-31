using Registry.Errors.Services;
using Registry.Models;
using System.Security.Claims;

namespace Registry.Identity
{
    public static class UserExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            try
            {
                var value = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == nameof(TokenRegistrationRequest.Id))?.Value ?? throw new UnauthorizedException();
                var userId = Guid.Parse(value);
                return userId;
            }
            catch (FormatException)
            {
                throw new ApplicationException("Invalid user");
            }
        }
    }
}
