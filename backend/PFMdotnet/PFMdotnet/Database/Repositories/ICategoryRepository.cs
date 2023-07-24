using PFMdotnet.Database.Entities;
using PFMdotnet.Models;

namespace PFMdotnet.Database.Repositories
{
    public interface ICategoryRepository
    {

        Task<AfterBulkAdd<CategoryEntity>> CreateBulk(List<CategoryEntity> categories);

        // Task<List<CategoryEntity>> GetAll();

        Task<CategoryEntity> FindByCode(string categoryCode);

        Task<List<CategoryEntity>> GetAnalyticsAsync(string categoryCode);

    }
}
