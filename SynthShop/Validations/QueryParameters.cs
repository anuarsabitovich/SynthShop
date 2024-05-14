using Microsoft.AspNetCore.Mvc;

namespace SynthShop.Validations
{
    public class QueryParameters
    {
        [FromQuery()]
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 1000;
    }
}

