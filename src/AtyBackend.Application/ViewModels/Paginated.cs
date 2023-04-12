namespace AtyBackend.Application.ViewModels;

public class Paginated<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
    //public string? FirstPageUrl { get; set; }
    //public string? LastPageUrl { get; set; }
    //public string? NextPageUrl { get; set; }
    //public string? PreviousPageUrl { get; set; }
    public List<T> Data { get; set; }

    public Paginated(int? pageNumber, int? pageSize, int totalItems, List<T> dtos)
    {
        pageNumber = pageNumber is null ? 1 : pageNumber.Value;
        pageSize = pageSize is null ? 20 : pageSize.Value;

        // colocar limite do page size baseado no appsettings.json
        // var pageSizeMaxLength = _configuration["ConnectionStrings:DefaultConnection"];
        // var test = pageSizeMaxLength is null ? true : false;
        int pageSizeMaxLength = 1000;
        pageSize = pageSize <= pageSizeMaxLength ? pageSize : pageSizeMaxLength;

        PageNumber = pageNumber.Value;
        PageSize = pageSize.Value;
        TotalItems = totalItems;
        TotalPages = (totalItems > 0 && totalItems < pageSize) ? 1 : (int)Math.Ceiling(totalItems / (double)pageSize);
        Data = dtos;

        //var previousPageNumber = pageNumber - 1;
        //var nextPageNumber = pageNumber + 1;

        //FirstPageUrl = $"{url}?pageNumber=1&pageSize={pageSize}";
        //LastPageUrl = $"{url}?pageNumber={TotalPages}&pageSize={pageSize}";
        //PreviousPageUrl = previousPageNumber < 1 ? null : $"{url}?pageNumber={previousPageNumber}&pageSize={pageSize}";
        //NextPageUrl = nextPageNumber > TotalPages ? null : $"{url}?pageNumber={nextPageNumber}&pageSize={pageSize}";
    }
}