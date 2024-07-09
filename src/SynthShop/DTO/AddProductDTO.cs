namespace SynthShop.DTO;

public class AddProductDTO
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public IFormFile Picture { get; set; }
    public Guid CategoryID { get; set; }
}