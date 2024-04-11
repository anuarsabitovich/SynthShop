﻿namespace SynthShop.Data.DTO
{
    public class OrderDTO
    {
        public Guid OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public Guid CustomerID { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
