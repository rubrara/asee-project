using PFMdotnet.Commands;
using PFMdotnet.Database.Entities;
using PFMdotnet.Database.Enums;
using PFMdotnet.Models;

namespace PFMdotnet.Services
{
    public interface ICategoryService
    {
        Task<AfterBulkAdd<CategoryEntity>> CreateCategoryBulk(List<CreateCategoryCommand> commands);

        Task<ReturnDTO<AnalyticsGroup>> GetAnalytics(AnalyticsSearchParams searchParams);
        Task<Category> GetCategory(string catCode);
    }
}
