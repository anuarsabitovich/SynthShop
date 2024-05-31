using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthShop.Domain.Entities
{
    public class Basket
    {
        public Guid BasketId { get; set; }
        public Guid? CustomerId { get; set; } 

        public List<BasketItem> Items { get; set; }
        
        public Basket()
        {
            Items = new List<BasketItem>();
        }


    }
}
