using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SynthShop.Domain.Entities
{
    public class BasketItem
    {
        public Guid BasketItemId { get; set; }
        public int Quantity { get; set; }
        public Guid ProductId {get; set; } 
        public Product Product { get; set; }
        
        public Guid? BasketId { get; set; }
        [JsonIgnore]
        public Basket? Basket { get; set; }
    }
}
