namespace SynthShop.Domain.Extensions
{
    public static class PagedListExtensions
    {
        public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int page, int pageSize) where T : class
        {
            var totalItems = source.Count();
            var items = source.Skip((page - 1) * pageSize).Take(pageSize);
            
            return new(items,totalItems, page, pageSize);
            
        }

    }
}
