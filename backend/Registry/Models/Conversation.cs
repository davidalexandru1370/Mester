namespace Registry.Models
{
    public class Conversation
    {
        public Guid Id { get; set; }
        public ClientJobRequest Request { get; set; } = default!;
        public required Guid RequestId { get; set; }
        public User TradesMan { get; set; }
        public required Guid TradesManId { get; set; }
        public List<Message> Messages { get; set; } = new();
        public List<TradesManJobResponse> Responses { get; set; } = new();

        // set if the tradesman doesn't want to honor the request
        public bool Declined { get; set; } = false;
    }
}
