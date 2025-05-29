namespace Registry.Models
{
    public class TradesManSpecialities
    {
        public Guid TradesManId { get; set; }
        public Guid SpecialityId { get; set; }
        public TradesMan TradesMan { get; set; }
        public Speciality Speciality { get; set; }
        public uint Price { get; set; }
        public string UnitOfMeasure { get; set; }
    }
}
