namespace Registry.DTO
{
    public class SpecialityDTO
    {
        public Guid SpecialityId { get; set; }
        public Guid TradesManId { get; set; }
        public string Type { get; set; }
        public string ImageUrl { get; set; }
        public uint Price { get; set; }
        public string UnitOfMeasure { get; set; }
    }
}
