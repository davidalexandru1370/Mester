using Microsoft.AspNetCore.Mvc;
using Registry.Services.Interfaces;

namespace Registry.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResponseController : ControllerBase
    {
        private readonly ITradesManService _serviceTradesMan;
        private readonly IJobsService _serviceJobs;
        private readonly IUserService _userService;

        public ResponseController(ITradesManService service, IUserService userService, IJobsService serviceJobs)
        {
            _serviceTradesMan = service;
            _userService = userService;
            _serviceJobs = serviceJobs;
        }

        [HttpPatch("{responseId}/accept")]
        public async Task<IActionResult> AcceptJobResponse(Guid responseId)
        {
            var user = await _userService.GetClientByClaims(User);
            await _serviceJobs.AcceptResponse(user, responseId);
            return Ok();
        }
    }
}
