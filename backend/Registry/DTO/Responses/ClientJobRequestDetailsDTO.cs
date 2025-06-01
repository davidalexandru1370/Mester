namespace Registry.DTO.Responses
{
    public class ClientJobRequestDetailsDTO
    {
        public required ClientJobRequestDTO Request { get; set; }
        public required List<ConversationWithLastOfferDTO> Conversations { get; set; }
    }
}
