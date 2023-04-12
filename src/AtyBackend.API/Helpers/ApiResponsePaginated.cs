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
            // colocar limite do page size baseado no appsettings.json
            // var pageSizeMaxLength = _configuration["ConnectionStrings:DefaultConnection"];
            // var test = pageSizeMaxLength is null ? true : false;
            int pageSizeMaxLength = 1000;

            pageNumber = pageNumber is null ? 1 : pageNumber.Value;
            pageSize = pageSize is null ? 20 : pageSize <= pageSizeMaxLength ? pageSize : pageSizeMaxLength;

            //pageSize = pageSize <= pageSizeMaxLength ? pageSize : pageSizeMaxLength;

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

            //paginatedResult.PreviousPageUrl = HasPreviousPage(paginated) ? GetPageUrl(paginated, url, false) : null;
            //paginatedResult.NextPageUrl = HasNextPage(paginated) ? GetPageUrl(paginated, url) : null;
        }

        //private static int GetTotalPages(double totalItems, double pageSize) => (int)Math.Ceiling(totalItems / pageSize);

        ////condição em que não tem não tem previous
        ////-> pageNumber = 1
        ////-> pageNumber > TotalPages
        ////-> pageNumber < 1
        //private static bool HasNextPage(Paginated<T> result) =>
        //    !(result.PageNumber == result.TotalPages || result.PageNumber > result.TotalPages || result.PageNumber < 1);

        ////condição em que não tem next
        ////-> pageNumber = TotalPages
        ////-> pageNumber > TotalPages
        ////-> pageNumber < 1
        //private static bool HasPreviousPage(Paginated<T> result) =>
        //    !(result.PageNumber == 1 || result.PageNumber > result.TotalPages || result.PageNumber < 1);

        //private static string GetPageUrl(Paginated<T> result, string url, bool isNextPage = true) => isNextPage ?
        //    url + "?pageNumber=" + (result.PageNumber + 1) + "&pageSize=" + result.PageSize :
        //    url + "?pageNumber=" + (result.PageNumber - 1) + "&pageSize=" + result.PageSize;


        // construtor só receber pageNumber e pageSize, validar/limitar esses dois parametros
        // ter um método para add os dados e URLs
    }
}
