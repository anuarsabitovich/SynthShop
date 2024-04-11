namespace SynthShop.Data.DTO
{
    public class UpdateOrderDTO
    {
        public DateTime OrderDate { get; set; }
        public Guid CustomerID { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
