namespace SynthShop.DTO
{
    public class CreateOrderDTO
    {
        public Guid CustomerId { get; set; }
        public Guid BasketId { get; set; }
    }
}
