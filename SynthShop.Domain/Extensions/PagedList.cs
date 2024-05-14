namespace SynthShop.Domain.Extensions
{
    public sealed class PagedList<T>
        where T : class
    {
        public IEnumerable<T> Items { get; }

        public int Page { get; }

        public int PageSize { get; }

        public int TotalItems { get; }

        public bool HasNextPage => Page * PageSize < TotalItems;

        public bool HasPreviousPage => Page - 1 is not 0;

        public PagedList(IEnumerable<T> items, int totalItems, int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
            TotalItems = totalItems;
            Items = items;
        }
    }
}
