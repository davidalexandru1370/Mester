namespace Registry.DTO.Responses
{
    public class MessageAndResponsesDTO
    {
        public required bool IsMe { get; set; }
        public required DateTime Sent { get; set; }
        public required DateTime? Seen { get; set; }
        public MessageResponse? Message { get; set; }
        public TradesManJobResponseDTO? ClientRequestResponse { get; set; }
    }
}
