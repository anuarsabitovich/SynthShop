using Microsoft.AspNetCore.Identity;

namespace SynthShop.Domain.Entities
{
    public class User : IdentityUser<Guid> 
    {
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
