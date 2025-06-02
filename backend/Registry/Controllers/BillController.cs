using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Registry.DTO.Responses;
using Registry.Services.Interfaces;

namespace Registry.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly ITradesManService _serviceTradesMan;
        private readonly IJobsService _serviceJobs;
        private readonly IUserService _userService;

        public BillController(ITradesManService service, IUserService userService, IJobsService serviceJobs)
        {
            _serviceTradesMan = service;
            _userService = userService;
            _serviceJobs = serviceJobs;
        }
        [Authorize]
        [HttpPatch("{billId}/pay")]
        public async Task<IActionResult> PayBill(Guid billId)
        {
            var user = await _userService.GetClientByClaims(User);
            BillDTO bill = await _serviceJobs.PayBill(user, billId);
            return Ok(bill);
        }
    }
}
