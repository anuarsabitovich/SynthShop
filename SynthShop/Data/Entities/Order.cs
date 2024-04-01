namespace SynthShop.Data.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
    }

}
