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

        private readonly IUserService _userService;
        public ConversationsController(ITradesManService service, IUserService userService)
        {
            _service = service;
            _userService = userService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetConversations()
        {
            var user = await _userService.GetByClaims(User);

            var conversations = await _service.GetConversations(user);

            return Ok(conversations);
        }

        [Authorize]
        [HttpPut("users/{WithUserId}")]
        public async Task<IActionResult> GetOrCreateConversation(Guid WithUserId)
        {
            var user = await _userService.GetByClaims(User);
            var conversation = await _service.GetOrCreateConversation(user, WithUserId);

            return Ok(conversation);
        }

        [Authorize]
        [HttpGet("{conversationId}")]
        public async Task<IActionResult> GetConversationMessages(Guid conversationId)
        {
            var user = await _userService.GetByClaims(User);

            var conversations = await _service.GetMessages(user, conversationId);

            return Ok(conversations);
        }

        [Authorize]
        [HttpPost("{conversationId}/Send")]
        public async Task<IActionResult> SendMessage(Guid conversationId, [FromBody] SendMessageRequest request)
        {
            var user = await _userService.GetByClaims(User);

            var r = await _service.SendMessage(user, conversationId, request);

            return Ok(r);

        }
    }
}
