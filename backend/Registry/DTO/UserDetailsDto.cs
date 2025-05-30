namespace Registry.DTO
{
    public class UserDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ImageUrl { get; set; }
        public bool IsTradesman { get; set; }
    }
}
