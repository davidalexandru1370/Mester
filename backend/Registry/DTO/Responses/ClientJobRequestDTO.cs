namespace Registry.DTO.Responses
{
    public class ClientJobRequestDTO
    {
        public required Guid Id { get; set; } = Guid.NewGuid();
        public required DateTime RequestedOn { get; set; }
        public DateTime? StartDate { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required bool ShowToEveryone { get; set; }
        public required bool Open { get; set; } = true;
        // TODO: change to require and remove the default when populated
        public List<string> ImagesUrl { get; set; } = new();
        public required Guid? JobApprovedId { get; set; }
    }
}
