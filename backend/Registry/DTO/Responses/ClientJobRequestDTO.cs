namespace Registry.DTO.Responses
{
    public class ClientJobRequestDTO
    {
        public required Guid Id { get; set; }
        public required DateTime RequestedOn { get; set; }
        public DateTime? StartDate { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required bool ShowToEveryone { get; set; }
        public required bool Open { get; set; }
        public required List<string> ImagesUrl { get; set; }
        public required Guid? JobApprovedId { get; set; }
    }
}
