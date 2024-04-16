namespace SynthShop.Domain.Entities
{
    public class Order
    {
        public Guid OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public Guid CustomerID { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsDeleted { get; set; }
        public Customer Customer { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }

    }
}
