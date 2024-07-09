using SynthShop.Domain.Enums;

namespace SynthShop.DTO;

public class OrderDTO
{
    public Guid OrderID { get; set; }
    public DateTime OrderDate { get; set; }
    public Guid UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public List<OrderItemDTO> OrderItems { get; set; }
}