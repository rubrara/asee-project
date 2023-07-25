using PFMdotnet.Models;

namespace PFMdotnet.Helpers.SearchReturnObjects.Categories
{
    public class CategoriesReturnDto
    {
        public string Message { get; set; }
        public string? Result { get; set; } = null;
        public CategoryDto? Category { get; set; } = null;
        public List<CategoryDto>? Categories { get; set; } = null;
        public List<string>? Errors { get; set; } = null;
    }
}
