using System.ComponentModel.DataAnnotations;

namespace SynthShop.Data.DTO
{
    public class UpdateCategoryDTO
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
