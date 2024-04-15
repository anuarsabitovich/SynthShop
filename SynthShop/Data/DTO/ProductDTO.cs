namespace SynthShop.Data.DTO
{
    public class ProductDTO
    {
        public Guid ProductID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public Guid CategoryID { get; set; }
    }
}
