using Registry.DTO.Responses;

namespace Registry.DTO
{
    public class ConversationDTO
    {
        public required Guid Id { get; set; }
        public required ClientJobRequestDTO ClientRequest { get; set; }
        public required ConversationUserDTO TradesMan { get; set; }
        public MessageOrResponsesDTO? LastMessage { get; set; }
    }
}
