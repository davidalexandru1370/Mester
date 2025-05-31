namespace Registry.DTO
{
    public class TradesManDTO
    {
        public required List<AssignSpecialityDTO> Specialities { get; set; }
        public required string Description { get; set; }
        public required string City { get; set; }
        public required string County { get; set; }
    }
}
