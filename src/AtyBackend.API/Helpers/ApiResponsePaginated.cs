using AtyBackend.Application.DTOs;
using AtyBackend.Application.ViewModels;

namespace AtyBackend.API.Helpers
{
    public class ApiResponsePaginated<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public string? FirstPageUrl { get; set; }
        public string? LastPageUrl { get; set; }
        public string? NextPageUrl { get; set; }
        public string? PreviousPageUrl { get; set; }
        public List<T>? Data { get; set; }

        public ApiResponsePaginated(int? pageNumber, int? pageSize)
        {
            int pageSizeMaxLength = 1000;

            pageNumber = pageNumber is null ? 1 : pageNumber.Value;
            pageSize = pageSize is null ? 10 : pageSize <= pageSizeMaxLength ? pageSize : pageSizeMaxLength;

            PageNumber = pageNumber.Value;
            PageSize = pageSize.Value;
        }

        public void AddData(Paginated<T> paginated, HttpRequest request)
        {
            TotalPages = paginated.TotalPages;
            TotalItems = paginated.TotalItems;
            Data = paginated.Data;

            var url = (request.IsHttps ? "https://" : "http://") + request.Host.ToString() + request.Path.ToString();
            var previousPageNumber = PageNumber - 1;
            var nextPageNumber = PageNumber + 1;

            FirstPageUrl = $"{url}?pageNumber=1&pageSize={PageSize}";
            LastPageUrl = $"{url}?pageNumber={TotalPages}&pageSize={PageSize}";
            PreviousPageUrl = previousPageNumber < 1 ? null : $"{url}?pageNumber={previousPageNumber}&pageSize={PageSize}";
            NextPageUrl = nextPageNumber > TotalPages ? null : $"{url}?pageNumber={nextPageNumber}&pageSize={PageSize}";
        }
    }
}
