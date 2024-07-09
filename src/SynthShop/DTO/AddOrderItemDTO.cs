namespace SynthShop.DTO;

public class AddOrderItemDTO
{
    public Guid OrderID { get; set; }
    public Guid ProductID { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}