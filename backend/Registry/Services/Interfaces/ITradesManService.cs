using Registry.DTO;
using Registry.DTO.Requests;
using Registry.DTO.Responses;
using Registry.Models;

namespace Registry.Services.Interfaces;

public interface ITradesManService
{
    Task<List<string>> GetSpecialities();
    Task<Speciality?> FindSpeciality(string Type);
    Task<List<Speciality>> GetSpecialitiesByName(IList<string> specialitiesTypeNames);
    Task UpdateTradesManProfile(User user, TradesManDTO tradesManDTO);
    Task<List<TradesManListDTO>> GetTradesManList(FilterListTradesMen filter);
    Task<TradesManInfoPageDTO?> GetTradesManInfo(Guid id);
    Task<Speciality> AddSpecialty(Speciality speciality);
    Task<List<Speciality>> AddSpecialitiesBulk(List<Speciality> specialities);
    Task<List<ConversationDTO>> GetConversations(User user);
    Task<List<MessageDTO>?> GetMessages(User user, Guid conversationId);
    Task<SendMessageResponse> SendMessage(User user, Guid conversationId, SendMessageRequest sendMessage);
    Task<ConversationDTO> GetOrCreateConversation(User user, Guid withUserId);
    Task<List<FindTradesManDTO>> FindTradesMen(string pattern, int limit);
}