using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Registry.DTO.Requests;
using Registry.Services.Interfaces;

namespace Registry.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationsController : ControllerBase
    {
        private readonly ITradesManService _service;
        private readonly IJobsService _serviceJobs;

        private readonly IUserService _userService;
        public ConversationsController(ITradesManService service, IUserService userService, IJobsService serviceJobs)
        {
            _service = service;
            _userService = userService;
            _serviceJobs = serviceJobs;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetConversations()
        {
            var user = await _userService.GetByClaims(User);

            var conversations = await _serviceJobs.GetConversations(user);

            return Ok(conversations);
        }

        [Authorize]
        [HttpPut("jobRequests/{clientJobRequestId}/tradesman/{tradesManId}")]
        public async Task<IActionResult> GetOrCreateConversation(Guid clientJobRequestId, Guid tradesManId)
        {
            var user = await _userService.GetByClaims(User);
            var conversation = await _serviceJobs.GetOrCreateConversation(user, clientJobRequestId, tradesManId);

            return Ok(conversation);
        }

        [Authorize]
        [HttpGet("{conversationId}")]
        public async Task<IActionResult> GetConversationMessages(Guid conversationId)
        {
            var user = await _userService.GetByClaims(User);

            var conversations = await _serviceJobs.GetMessages(user, conversationId);

            return Ok(conversations);
        }

        [Authorize]
        [HttpPost("{conversationId}/Send")]
        public async Task<IActionResult> SendMessage(Guid conversationId, [FromBody] SendMessageRequest request)
        {
            var user = await _userService.GetByClaims(User);

            var r = await _serviceJobs.SendMessage(user, conversationId, request);

            return Ok(r);

        }
    }
}
