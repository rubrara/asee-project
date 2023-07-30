namespace PFMdotnet.Helpers.SearchReturnObjects.Categories
{
    public class SearchCategoriesParams
    {
        public string? ParentId { get; set; } 
        public int? Page { get; set; } 
        public int? PageSize { get; set; } 
        public string? SortBy { get; set; } 
        public string? SortOrder { get; set; } 

    }
}
