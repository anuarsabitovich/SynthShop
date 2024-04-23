namespace SynthShop.DTO
{
    public class OrderDTO
    {
        public Guid OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public Guid CustomerID { get; set; }
        public decimal TotalAmount { get; set; }
        
        public List<OrderItemDTO> OrderItems {get; set; }
    }
}
