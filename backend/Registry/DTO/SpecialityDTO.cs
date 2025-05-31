namespace Registry.DTO
{
    public class SpecialityDTO
    {
        public required Guid SpecialityId { get; set; }
        public required Guid TradesManId { get; set; }
        public required string Type { get; set; }
        public required string ImageUrl { get; set; }
        public required uint Price { get; set; }
        public required string UnitOfMeasure { get; set; }
    }
}
