namespace ModelService
{
    public class PaginationResponse<T> where T : class
    {
        public PaginationDetails Page { get; set; }
        public SortDetails Sort { get; set; }
        public FilterDetails Filter { get; set; }
        public T Items { get; set; }
        public PaginationResponse(T items, PaginationDetails page, SortDetails sort, FilterDetails filter)
        {
            Items = items;
            Page = page;
            Sort = sort;
            Filter = filter;
        }
    }

    public class PaginationDetails
    {
        public int RecordCount { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
        public int PageCount { get; set; }
    }

    public class SortDetails
    {
        public string? By { get; set; }
        public string? Order { get; set; }
    }

    public class FilterDetails
    {
        public string? By { get; set; }
        public string? Query { get; set; }
    }

    public class PaginationRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? SortColumn { get; set; }
        public string? SortOrder { get; set; }
        public string? FilterColumn { get; set; }
        public string? FilterValue { get; set; }
    }

    public class ResponseModel
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
    }

    public class ProductFilter
    {
        public List<string>? Brands { get; set; }
        public List<string>? Categories { get; set; }
        public string? MinPriceRange  { get; set; }
        public string? MaxPriceRange  { get; set; }
    }
}