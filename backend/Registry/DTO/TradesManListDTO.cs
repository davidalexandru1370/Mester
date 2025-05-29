using Registry.Models;

namespace Registry.DTO
{
    public class TradesManListDTO
    {
        // TODO: add profile picture
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public required List<SpecialityDTO> Specialities { get; set; }
    }
}
