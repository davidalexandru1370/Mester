using Microsoft.AspNetCore.Mvc;
using Registry.Services.Interfaces;

namespace Registry.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController
    {
        private readonly ITradesManService _service;

        private readonly IUserService _userService;
        public JobController(ITradesManService service, IUserService userService)
        {
            _service = service;
            _userService = userService;
        }
    }
}
