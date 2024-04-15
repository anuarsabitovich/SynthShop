namespace SynthShop.Data.DTO
{
    public class OrderItemDTO
    {
        public Guid OrderItemID { get; set; }
        public Guid OrderID { get; set; }
        public Guid ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
