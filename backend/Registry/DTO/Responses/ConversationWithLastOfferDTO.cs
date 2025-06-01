namespace Registry.DTO.Responses
{
    public class ConversationWithLastOfferDTO
    {
        public required Guid Id { get; set; }
        public required ConversationUserDTO TradesMan { get; set; }
        public required TradesManJobResponseDTO? Response { get; set; }
    }
}
