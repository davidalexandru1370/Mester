using Registry.DTO;
using Registry.DTO.Requests;
using Registry.DTO.Responses;
using Registry.Models;

namespace Registry.Services.Interfaces
{
    public interface IJobsService
    {
        Task AcceptResponse(User user, Guid requestId, Guid responseId);
        Task<TradesManJobResponseDTO> AddTradesManJobResponse(User tradesMan, CreateTradesManJobResponse request);
        Task<Guid> CreateClientRequest(User user, CreateClientJobRequest request);
        Task EndJob(User tradesMan, Guid jobId, DateTime endDate);
        Task<List<ConversationDTO>> GetConversations(User user);
        Task<List<MessageAndResponsesDTO>?> GetMessages(User user, Guid conversationId);
        Task<ConversationDTO> GetOrCreateConversation(User user, Guid clientJobRequestId, Guid tradesManId);
        Task<ConversationDTO> SendClientRequestToConversation(User client, Guid clientRequestId, Guid tradesManId);
        Task<SendMessageResponse> SendMessage(User user, Guid conversationId, SendMessageRequest sendMessage);
        Task UpdateClientRequest(User user, Guid clientRequestId, UpdateClientJobRequest request);
    }
}