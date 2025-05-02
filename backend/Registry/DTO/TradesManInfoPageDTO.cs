namespace Registry.DTO
{
    public class TradesManInfoPageDTO
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required List<string> Specialities { get; set; }

        // TODO: add profile picture, add photo carousel, reviews
    }
}
