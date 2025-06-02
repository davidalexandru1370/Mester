namespace Registry.DTO.Requests
{
    public class UpdateClientJobRequest
    {
        public DateTime? StartDate { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool? ShowToEveryone { get; set; }
        public bool? Open { get; set; } = true;
        public List<string>? ImagesBase64 { get; set; }
    }
}
