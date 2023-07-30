namespace PFMdotnet.Models.Categories
{
    public class CategoriesPagedList<T>
    {
        public string? Message { get; set; } = null;
        public int? TotalCount { get; set; } = null;

        public int? PageSize { get; set; } = null;

        public int? Page { get; set; } = null;

        public int? TotalPages { get; set; } = null;

        public List<T>? Items { get; set; } = null;

        public List<string>? Errors { get; set; } = null;
    }
}
