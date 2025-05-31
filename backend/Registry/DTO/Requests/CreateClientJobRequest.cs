namespace Registry.DTO.Requests
{
    public class CreateClientJobRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime RequestedOn { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public bool ShowToEveryone { get; set; }
    }
}
