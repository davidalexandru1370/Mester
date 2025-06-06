﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Registry.DTO.Requests;
using Registry.DTO.Responses;
using Registry.Errors.Services;
using Registry.Services.Interfaces;

namespace Registry.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestsController : ControllerBase
    {
        private readonly ITradesManService _serviceTradesMan;
        private readonly IJobsService _serviceJobs;
        private readonly IUserService _userService;

        public RequestsController(ITradesManService service, IUserService userService, IJobsService serviceJobs)
        {
            _serviceTradesMan = service;
            _userService = userService;
            _serviceJobs = serviceJobs;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetRequestsClient()
        {
            var user = await _userService.GetByClaims(User);
            var response = await _serviceJobs.AllClientRequests(user);
            return Ok(response);
        }

        [Authorize]
        [HttpGet("{requestId}/conversations")]
        public async Task<IActionResult> GetRequestClient(Guid requestId)
        {
            var user = await _userService.GetByClaims(User);
            var response = await _serviceJobs.GetConversationsLastOffer(user, requestId);
            return Ok(response);
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
            var r = await _serviceJobs.UpdateClientRequest(user, clientRequestId, request);
            return Ok(r);
        }

        [Authorize]
        [HttpPost("{clientRequestId}/send/tradesmen/{tradesManId}")]
        public async Task<IActionResult> SendClientRequest(Guid clientRequestId, Guid tradesManId)
        {
            var user = await _userService.GetByClaims(User);
            var r = await _serviceJobs.SendClientRequestToTradesMan(user, clientRequestId, tradesManId);
            return Ok(r);
        }

        [Authorize]
        [HttpGet("global")]
        public async Task<IActionResult> GetGlobalRequest()
        {
            var user = await _userService.GetByClaims(User);
            if (user.TradesManProfile is null) throw new UnauthorizedException();
            List<ClientJobRequestDTO> requests = await _serviceJobs.GetGlobalRequests(user);
            return Ok(requests);
        }
    }
}
