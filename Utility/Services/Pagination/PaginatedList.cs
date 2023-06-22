using Microsoft.EntityFrameworkCore;

namespace Utility.Services.Pagination
{
    public class PaginatedList<T> : List<T>
    {
        public PaginatedList(List<T> items, int totalCount, int pageIndex, int pageSize)
        {
            this.AddRange(items);
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        }

        public int TotalPages { get; set; }
        public int PageIndex { get; set; }

        public bool HasPreviousPage => PageIndex > 1;

        public bool HasNextPage => PageIndex < TotalPages;

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            int totalCount = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, totalCount, pageIndex, pageSize);
        }
    }
}
