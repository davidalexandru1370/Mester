using Microsoft.AspNetCore.Mvc;
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

        //TODO: maybe it's not in the right place
        //[Authorize]
        //public async Task<IActionResult> TradesManGetOrCreateConversation(Guid clientJobRequestId)
        //{
        //    var tradesMan = await _userService.GetByClaims(User);
        //    if (tradesMan.TradesManProfile is null)
        //    {
        //        throw new UnauthorizedException();
        //    }
        //    await _serviceJobs.GetOrCreateConversation(tradesMan, clientJobRequestId);
        //    return Ok();
        //}
    }
}
