using Microsoft.AspNetCore.Mvc;
using Registry.DTO;
using Registry.Services;

namespace Registry.Controllers
{
    public record ListRequest(FilterListTradesMen filter);
    [Route("api/[controller]")]
    [ApiController]
    public class TradesManController : ControllerBase
    {
        private TradesManService _service;

        public TradesManController(TradesManService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult> List([FromBody] ListRequest request)
        {
            List<TradesManListDTO> r = await _service.GetTradesManList(request.filter);
            return Ok(r);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(Guid id)
        {
            var r = await _service.GetTradesManInfo(id);
            return Ok(r);
        }
    }
    
    
}
