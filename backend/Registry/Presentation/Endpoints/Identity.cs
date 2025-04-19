using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Registry.Errors.Services;
using Registry.Models;
using Registry.Services;
using System.Text.Json;

namespace Registry.Endpoints
{
    public static class Identity
    {
        public record CreateAccountRequest(string Email, string Password);
        public record LoginRequest(string Email, string Password);

        public static IEndpointRouteBuilder UseIdentityRoutes(this IEndpointRouteBuilder builder)
        {

            var group = builder
                .MapGroup("/identity");
            group.MapPost("/createAccount", CreateAccount);
            group.MapPost("/login", Login);

            //group.AllowAnonymous();

            return builder;
        }

        static async Task<Results<Ok, Conflict>> CreateAccount(AuthenticationService service, [FromBody] CreateAccountRequest request)
        {
            try
            {
                await service.CreateUser(request.Email, request.Password);
                return TypedResults.Ok();
            }
            catch (NameAlreadyUsedException)
            {
                return TypedResults.Conflict();
            }
        }

        static async Task<Results<Ok<TokenResponse>, UnauthorizedHttpResult>> Login(AuthenticationService service, [FromForm(Name = "email")] string username, [FromForm] string password)
        {
            var user = await service.LoginUser(username, password);
            if (user is null)
            {
                return TypedResults.Unauthorized();
            }
            var token = service.CreateToken(new TokenRegistrationRequest
            {
                Email = username,
                CustomClaims = new Dictionary<string, JsonElement>()
            });
            return TypedResults.Ok(token);
        }
    }
}
