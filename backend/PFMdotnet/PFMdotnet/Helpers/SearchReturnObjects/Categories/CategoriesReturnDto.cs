using PFMdotnet.Models;

namespace PFMdotnet.Helpers.SearchReturnObjects.Categories
{
    public class CategoriesReturnDto
    {
        public string Message { get; set; }
        public string? Result { get; set; } = null;
        public Category? Category { get; set; } = null;
        public List<Category>? Categories { get; set; } = null;
        public List<string>? Errors { get; set; } = null;
    }
}
