namespace SynthShop.Domain.Entities;

public class RefreshToken
{
    public Guid Token { get; set; }
    public string JwtId { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool Used { get; set; }
    public bool Invalidated { get; set; }
    public Guid UserId { get; set; }

    public User User { get; set; }
}