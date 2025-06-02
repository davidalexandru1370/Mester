using Registry.DTO;
using Registry.DTO.Requests;
using Registry.DTO.Responses;
using Registry.Models;

namespace Registry.Services.Interfaces
{
    public interface IJobsService
    {
        Task AcceptResponse(User user, Guid responseId);
        Task<TradesManJobResponseDTO> AddTradesManJobResponse(User tradesMan, Guid conversationId, CreateTradesManJobResponse request);
        Task<List<ClientJobRequestDTO>> AllClientRequests(User user);
        Task<List<ConversationWithLastOfferDTO>> GetConversationsLastOffer(User user, Guid jobRequestDetails);
        Task<ClientJobRequestDTO> CreateClientRequest(User user, CreateClientJobRequest request);
        Task EndJob(User tradesMan, Guid jobId, DateTime endDate);
        Task<List<ConversationDTO>> GetConversations(User user);
        Task<List<MessageOrResponsesDTO>> GetMessages(User user, Guid conversationId);
        /// <summary>
        /// This function should be called only by the tradesman if he wants to responde to a global request
        /// </summary>
        Task<ConversationDTO> GetOrCreateConversation(User tradesManId, Guid clientJobRequestId);
        Task<ConversationDTO> SendClientRequestToTradesMan(User client, Guid clientRequestId, Guid tradesManId);
        Task<SendMessageResponse> SendMessage(User user, Guid conversationId, SendMessageRequest sendMessage);
        Task<ClientJobRequestDTO> UpdateClientRequest(User user, Guid clientRequestId, UpdateClientJobRequest request);
        Task<List<ClientJobRequestDTO>> GetGlobalRequests(User tradesMan);
    }
}