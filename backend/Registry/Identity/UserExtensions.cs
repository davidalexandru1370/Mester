using System.Security.Claims;

namespace Registry.Identity
{
    public static class UserExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal is null)
            {
                throw new ApplicationException("Invalid user");
            }

            try
            {
                var userId = Guid.Parse(claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "Id").Value);
                return userId;
            }
            catch (FormatException)
            {
                throw new ApplicationException("Invalid user");
            }
        }
    }
}
