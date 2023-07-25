using PFMdotnet.Database.Entities;
using PFMdotnet.Models;

namespace PFMdotnet.Database.Repositories
{
    public interface ICategoryRepository
    {

        Task<AfterBulkAdd<Category>> CreateBulk(List<Category> categories, int chunkSize);

        Task<List<Category>> GetCategoriesAsync(string parentId);

        Task<Category?> FindByCode(string categoryCode);

        Task<List<Category>> GetAnalyticsAsync(string categoryCode);

    }
}
