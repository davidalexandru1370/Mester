using Microsoft.AspNetCore.Cors;
using Microsoft.IdentityModel.Tokens;
using Registry.Errors.Repositories;
using Registry.Errors.Services;
using Registry.Models;
using Registry.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Registry.Services.Interfaces;
using Registry.DTO;

namespace Registry.Services;

[EnableCors("allPolicy")]
public class UserService : IUserService
{
    private readonly string tokenSecret;

    private readonly TimeSpan tokenLifetime;
    private readonly IRepositoryUser _repoUsers;

    public UserService(IRepositoryUser repoUsers, string tokenSecret, TimeSpan tokenLifetime)
    {
        this.tokenSecret = tokenSecret;
        this.tokenLifetime = tokenLifetime;
        _repoUsers = repoUsers;
    }

    public async Task<User?> GetByClaims(ClaimsPrincipal claims)
    {
        var email = claims.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        if (email is null)
        {
            return null;
        }
        var user = await _repoUsers.FindByEmail(email);
        return user;
    }

    public TokenResponse CreateToken(TokenRegistrationRequest request)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(tokenSecret);

        var claims = new List<Claim> {
            new(JwtRegisteredClaimNames.Email, request.Email),
        };

        foreach (var claimPair in request.CustomClaims)
        {
            var valueType = claimPair.Value.ValueKind switch
            {
                JsonValueKind.True => ClaimValueTypes.Boolean,
                JsonValueKind.False => ClaimValueTypes.Boolean,
                JsonValueKind.Number => ClaimValueTypes.Double,
                JsonValueKind.String => ClaimValueTypes.String,
                _ => throw new Exception($"Invalid claim {claimPair.Value}!")
            };
            var claim = new Claim(claimPair.Key, claimPair.Value.ToString(), valueType);
            claims.Add(claim);
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(tokenLifetime),
            //Issuer = "https://id.terec.com",
            //Audience = "https://stats.terec.com",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), "HS256")
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        var jwt = tokenHandler.WriteToken(token);
        return new TokenResponse(jwt, (int)tokenLifetime.TotalSeconds);
    }

    public async Task<User> CreateUser(string username, string password, string phoneNumber, string email)
    {
        var salt = Guid.NewGuid().ToString();

        var passwordHashed = GetHashedPassword(password, salt);
        var user = new User
        {
            HashPassword = passwordHashed,
            Email = email,
            Salt = salt,
            Name = username,
            PhoneNumber = phoneNumber,
        };
        try
        {
            return await _repoUsers.Add(user);
        }
        catch (ConflictException e)
        {
            throw new NameAlreadyUsedException(e);
        }
        catch (RepositoryException e)
        {
            throw new ServiceException(e);
        }
    }

    private static string GetHashedPassword(string password, string salt)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        return GetHashedPassword(passwordBytes, salt);
    }

    private static string GetHashedPassword(byte[] password, string salt)
    {
        using SHA256 sha256 = SHA256.Create();
        sha256.TransformBlock(password, 0, password.Length, null, 0);
        var saltBytes = Encoding.UTF8.GetBytes(salt);
        sha256.TransformFinalBlock(saltBytes, 0, saltBytes.Length);
        var hashed = Convert.ToBase64String(sha256.Hash!);
        return hashed;
    }

    /// <summary>
    /// Returns the user if the login is valid or null otherwise
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task<User?> LoginUser(string email, string password)
    {
        var user = await _repoUsers.FindByEmail(email);
        if (user is null)
        {
            return null;
        }
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        if (user.HashPassword != GetHashedPassword(passwordBytes, user.Salt))
        {
            return null;
        }
        return user;
    }

    public async Task<UserDetailsDto> GetUserDetailsById(Guid userId)
    {
        var user = await _repoUsers.GetUserById(userId);
        return new UserDetailsDto
        {
            Name = user.Name,
            Id = user.Id,
            IsTradesman = !(user.TradesManProfile is null)
        };
    }
}
