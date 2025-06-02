namespace Registry.DTO.Responses
{
    public class MessageOrResponsesOrBillDTO
    {

        public required DateTime Sent { get; set; }
        public required DateTime? Seen { get; set; }
        public MessageResponse? Message { get; set; }
        public TradesManJobResponseDTO? Response { get; set; }
        public BillDTO? Bill { get; set; }
    }
}
