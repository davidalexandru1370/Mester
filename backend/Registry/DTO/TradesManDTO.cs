namespace Registry.DTO
{
    public class TradesManDTO
    {
        public required List<AssignSpecialityDTO> Specialities { get; set; }
        public required string Description { get; set; }
        public string City { get; set; }
        public string County { get; set; }
    }
}
