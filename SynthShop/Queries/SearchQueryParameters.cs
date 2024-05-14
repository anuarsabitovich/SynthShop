using Microsoft.AspNetCore.Mvc;

namespace SynthShop.Queries
{
    public class SearchQueryParameters
    {
        [FromQuery(Name = "pageNumber")]
        public required int PageNumber { get; set; } = 1;

        [FromQuery(Name = "pageSize")]
        public int PageSize { get; set; }

        [FromQuery(Name = "searchTerm")]
        public string? SearchTerm { get; set; }

        [FromQuery(Name = "sortBy")]
        public string? SortBy { get; set; }

        [FromQuery(Name = "isAscending")]
        public bool? IsAscending { get; set; }
    }

}

