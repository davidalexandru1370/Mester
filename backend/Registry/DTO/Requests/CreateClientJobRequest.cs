namespace Registry.DTO.Requests
{
    public class CreateClientJobRequest
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public required bool ShowToEveryone { get; set; }
        public required List<string> ImagesBase64 { get; set; }
    }
}
