using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Registry.DTO;
using Registry.Errors.Services;
using Registry.Identity;
using Registry.Models;
using Registry.Models.Request;
using Registry.Services.Interfaces;

namespace Registry.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _authenticationService;
    private readonly ITradesManService _tradesManService;
    private readonly IDataSeedingService _dataSeedingService;

    public UserController(IUserService authenticationService, ITradesManService tradesManService, IDataSeedingService dataSeedingService)
    {
        _authenticationService = authenticationService;
        _tradesManService = tradesManService;
        _dataSeedingService = dataSeedingService;
    }
    
    [HttpPost("createAccount")]
    public async Task<Results<Ok, Conflict>> CreateAccount([FromBody] CreateAccountRequest request)
    {
        try
        {
            await _authenticationService.CreateUser(request.Email, request.Password, request.PhoneNumber, request.Email);
            return TypedResults.Ok();
        }
        catch (NameAlreadyUsedException)
        {
            return TypedResults.Conflict();
        }
    }
    
    [HttpPost("login")]
    public async Task<Results<Ok<TokenResponse>, UnauthorizedHttpResult>> Login([FromBody] LoginUserRequest loginUserData)
    {
        var user = await _authenticationService.LoginUser(loginUserData.Email, loginUserData.Password);
        if (user is null)
        {
            return TypedResults.Unauthorized();
        }

        var token = _authenticationService.CreateToken(new TokenRegistrationRequest
        {
            Email = loginUserData.Email,
            CustomClaims = []
        });
        return TypedResults.Ok(token);
    }

    [HttpPost("seed")]
    public async Task<IActionResult> Seed()
    {
        try
        {
            await _dataSeedingService.GenerateData();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }

        return Created();
    }
    
    [Authorize]
    [HttpPost("createTradesManProfile")]
    public async Task<Results<Ok, UnauthorizedHttpResult, Conflict, BadRequest<string>>> CreateTradesManProfile([FromBody] TradesManDTO tradesManDto)
    {
        var user = await _authenticationService.GetByClaims(this.User);
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
            await _tradesManService.UpdateTradesManProfile(user, tradesManDto);
        }
        catch (InvalidSpecialitiesTypeException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }

        return TypedResults.Ok();
    }

    [Authorize]
    [HttpGet("info")]
    public async Task<ActionResult<UserDetailsDto>> GetUserDetails()
    {
        var userId = User.GetUserId();
        var foundUser = await _authenticationService.GetUserDetailsById(userId);

        return Ok(foundUser);
    }
    
    [Authorize]
    [HttpGet("authorize")]
    public IActionResult Authorize()
    {
        return Ok();
    }
}