using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Registry.DTO;
using Registry.Errors.Services;
using Registry.Models;
using Registry.Services;
using System.Security.Claims;

namespace Registry.Endpoints
{
    public static class Identity
    {
        public record CreateAccountRequest(string Email, string Password, string PhoneNumber);
        public record LoginRequest(string Email, string Password);

        public static IEndpointRouteBuilder UseIdentityRoutes(this IEndpointRouteBuilder builder)
        {

            var group = builder
                .MapGroup("/identity")
                .DisableAntiforgery();
            group.MapPost("/createAccount", CreateAccount);
            group.MapPost("/login", Login);
            group.MapPost("/createTradesManProfile", CreateTradesManProfile).RequireAuthorization();


            return builder;
        }

        static async Task<Results<Ok, Conflict>> CreateAccount(AuthenticationService service, [FromForm] CreateAccountRequest request)
        {
            try
            {
                await service.CreateUser(request.Email, request.Password, request.PhoneNumber);
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
                CustomClaims = []
            });
            return TypedResults.Ok(token);
        }

        static async Task<Results<Ok, UnauthorizedHttpResult, Conflict, BadRequest<string>>> CreateTradesManProfile(AuthenticationService authenticationService, TradesManService service, ClaimsPrincipal claims, [FromBody] TradesManDTO tradesManDTO)
        {
            var user = await authenticationService.GetByClaims(claims);
            if (user is null)
            {
                return TypedResults.Unauthorized();
            }

            if (user.TradesManProfile is not null)
            {
                return TypedResults.Conflict();
            }
            try
            {
                await service.UpdateTradesManProfile(user, tradesManDTO);
            }
            catch (InvalidSpecialitiesTypeException ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }

            return TypedResults.Ok();
        }
    }
}
