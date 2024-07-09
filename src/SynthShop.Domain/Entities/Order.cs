using SynthShop.Domain.Enums;

namespace SynthShop.Domain.Entities;

public class Order
{
    public Guid OrderID { get; set; }
    public DateTime OrderDate { get; set; }
    public Guid UserId { get; set; }
    public bool IsDeleted { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public User User { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdateAt { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; }
}