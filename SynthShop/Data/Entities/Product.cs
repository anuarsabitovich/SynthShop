using System.ComponentModel;

namespace SynthShop.Data.Entities
{
    public class Product
    {
        public Guid ProductID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price {  get; set; }
        public int StockQuantity { get; set; }
        public Guid CategoryID { get; set; }
        public bool IsDeleted { get; set; }
        public Category Category { get; set;}

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
