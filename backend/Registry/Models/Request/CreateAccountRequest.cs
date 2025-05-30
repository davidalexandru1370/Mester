namespace Registry.Models.Request;

public class CreateAccountRequest
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; }
}