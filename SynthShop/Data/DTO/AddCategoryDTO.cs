using System.ComponentModel.DataAnnotations;

namespace SynthShop.Data.DTO
{
    public class AddCategoryDTO
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public string Description { get; set; }

    }
}
