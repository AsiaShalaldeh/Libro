using System.Text.Json;

namespace Libro.WebAPI.Helpers
{
    public static class PaginationHelper
    {
        public static void SetPaginationHeaders(HttpResponse response, int totalCount, int pageNumber, int pageSize)
        {
            var paginationHeader = new
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationHeader));
        }
    }
}
