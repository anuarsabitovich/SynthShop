namespace SynthShop.Domain.Extensions
{
    public static class PagedListExtensions
    {
        public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int page, int pageSize) where T : class
        {
            var totalItems = source.Count();
            var items = source.Skip((page - 1) * pageSize).Take(pageSize).ToList(); 
            return new PagedList<T>(items, totalItems, page, pageSize);
        }
    }
}