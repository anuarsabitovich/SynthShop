namespace SynthShop.Domain.Entities
{
    public class Category
    {
        public Guid CategoryID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
