using PFMdotnet.Database.Entities;
using PFMdotnet.Helpers.SearchReturnObjects.Categories;
using PFMdotnet.Models;

namespace PFMdotnet.Database.Repositories
{
    public interface ICategoryRepository
    {

        Task<AfterBulkAdd<Category>> CreateBulk(List<Category> categories, int chunkSize);

        Task<List<Category>> GetCategoriesAsync(FilterCategoriesParams searchParams);

        Task<Category?> FindByCode(string categoryCode);

        Task<List<Category>> GetAnalyticsAsync(string categoryCode);
        Task SaveChangesAsync();
    }
}
