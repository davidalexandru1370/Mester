namespace Registry.DTO.Responses
{
    public class MessageOrResponsesDTO
    {
        public required bool IsMe { get; set; }
        public required DateTime Sent { get; set; }
        public required DateTime? Seen { get; set; }
        public MessageResponse? Message { get; set; }
        public TradesManJobResponseDTO? ClientRequestResponse { get; set; }
    }
}
