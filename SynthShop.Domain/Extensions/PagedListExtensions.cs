namespace SynthShop.Domain.Extensions
{
    public static class PagedListExtensions
    { 
        public static PagedList<T> ToPagedList<T>(this IEnumerable<T> items, int page, int pageSize) where T : class => new(items, items.Count(), page, pageSize);
    }
}
