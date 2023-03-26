namespace AtyBackend.Application.ViewModels;

public class Paginated<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
    public string? NextPageUrl { get; set; }
    public string? PreviousPageUrl { get; set; }
    public IEnumerable<T> Data { get; set; }
}