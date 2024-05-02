namespace SynthShop.DTO;

public class RefreshTokenRequest
{
    public string Token { get; set; }
    public Guid RefreshToken { get; set; }
}