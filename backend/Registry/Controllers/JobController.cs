using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Registry.DTO.Requests;
using Registry.Errors.Services;
using Registry.Services.Interfaces;

namespace Registry.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly ITradesManService _serviceTradesMan;
        private readonly IJobsService _serviceJobs;
        private readonly IUserService _userService;

        public JobController(ITradesManService service, IUserService userService, IJobsService serviceJobs)
        {
            _serviceTradesMan = service;
            _userService = userService;
            _serviceJobs = serviceJobs;
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateClientRequest(CreateClientJobRequest request)
        {
            var user = await _userService.GetByClaims(User);
            var r = await _serviceJobs.CreateClientRequest(user, request);
            return Ok(r);
        }

        [Authorize]
        [HttpPatch("{clientRequestId}")]
        public async Task<IActionResult> UpdateClientRequest(Guid clientRequestId, [FromBody] UpdateClientJobRequest request)
        {
            var user = await _userService.GetByClaims(User);
            await _serviceJobs.UpdateClientRequest(user, clientRequestId, request);
            return Ok();
        }

        [Authorize]
        public async Task<IActionResult> SendClientRequest(Guid clientRequestId, Guid tradesManId)
        {
            var user = await _userService.GetByClaims(User);
            var r = await _serviceJobs.SendClientRequestToTradesMan(user, clientRequestId, tradesManId);
            return Ok(r);
        }

        //TODO: maybe it's not in the right place
        [Authorize]
        public async Task<IActionResult> TradesManGetOrCreateConversation(Guid clientJobRequestId)
        {
            var tradesMan = await _userService.GetByClaims(User);
            if (tradesMan.TradesManProfile is null)
            {
                throw new UnauthorizedException();
            }
            await _serviceJobs.GetOrCreateConversation(tradesMan, clientJobRequestId);
            return Ok();
        }


        [Authorize]
        [HttpGet("/requests")]
        public async Task<IActionResult> GetRequestsClient()
        {
            var user = await _userService.GetByClaims(User);
            var response = await _serviceJobs.AllClientRequests(user);
            return Ok(response);
        }

        [Authorize]
        [HttpGet("/requests/{requestId}")]
        public async Task<IActionResult> GetRequestClient(Guid requestId)
        {
            var user = await _userService.GetByClaims(User);
            var response = await _serviceJobs.GetConversationsLastOffer(user, requestId);
            return Ok(response);
        }
    }
}
