namespace Registry.DTO.Requests;

public class CreateAccountRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; }
}