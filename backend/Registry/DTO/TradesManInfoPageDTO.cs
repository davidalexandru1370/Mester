namespace Registry.DTO
{
    public class TradesManInfoPageDTO
    {
        public required Guid Id { get; set; }
        public required string ImageUrl {get;set;}
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string City { get; set; }
        public required string County { get; set; }
        public required List<SpecialityDTO> Specialities { get; set; }
        public required List<ImageDTO> Images { get; set; }

        // TODO: add profile picture, add photo carousel, reviews
    }
}
