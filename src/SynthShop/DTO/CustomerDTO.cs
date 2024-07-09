using SynthShop.Domain.Entities;

namespace SynthShop.DTO;

public class CustomerDTO
{
    public Guid CustomerID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public bool IsDeleted { get; set; }

    public ICollection<Order> Orders { get; set; }
}