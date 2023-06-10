namespace Libro.Domain.Common
{
    public class PaginatedResult<T>
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }
        public IEnumerable<T> Items { get; set; }

        public PaginatedResult(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public static async Task<PaginatedResult<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var totalCount = await Task.FromResult(source.Count());
            var items = await Task.FromResult(source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList());

            return new PaginatedResult<T>(items, totalCount, pageNumber, pageSize);
        }
    }
}
