namespace SynthShop.DTO;

public class RegisterUserDTO
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
}