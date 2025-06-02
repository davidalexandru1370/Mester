using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Registry.DTO.Requests;
using Registry.DTO.Responses;
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
        [HttpPost("{jobId}/bills")]
        public async Task<IActionResult> AddBill(Guid jobId, CreateBillRequest bill)
        {
            var tradesMan = await _userService.GetTradesManByClaims(User);
            BillDTO b = await _serviceJobs.AddBill(tradesMan, jobId, bill);
            return Ok(b);
        }
    }
}
