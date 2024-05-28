using Microsoft.EntityFrameworkCore;

namespace MVC_Music.Utilities
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        // Constructor to initialize the paginated list
        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        // Property to check if there is a previous page
        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        // Property to check if there is a next page
        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        // Creates a paginated list asynchronously
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

            // Adjust pageIndex if the current page has no items but there are items in previous pages
            if (items.Count() == 0 && count > 0 && pageIndex > 1)
            {
                pageIndex--;
                items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            }

            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
