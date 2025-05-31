namespace Registry.DTO
{
    public class FindTradesManDTO
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string? ImageUrl { get; set; }
    }
}
