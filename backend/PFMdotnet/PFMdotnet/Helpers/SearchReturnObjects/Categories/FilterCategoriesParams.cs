namespace PFMdotnet.Helpers.SearchReturnObjects.Categories
{
    public class FilterCategoriesParams
    {
        public string? ParentId { get; set; } 
        public int Page { get; set; } 
        public int PageSize { get; set; }
        public string? SortBy { get; set; } = "code";
        public string? SortOrder { get; set; } = "asc";
    }
}
