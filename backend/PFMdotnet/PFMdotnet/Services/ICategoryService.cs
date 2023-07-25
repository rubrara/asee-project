using PFMdotnet.Commands;
using PFMdotnet.Database.Entities;
using PFMdotnet.Helpers.SearchReturnObjects.Analytics;
using PFMdotnet.Helpers.SearchReturnObjects.Categories;
using PFMdotnet.Models;
using PFMdotnet.Models.Analytics;

namespace PFMdotnet.Services
{
    public interface ICategoryService
    {
        Task<AfterBulkAdd<CategoryEntity>> CreateCategoryBulk(List<CreateCategoryCommand> commands);

        Task<CategoriesReturnDto> GetCategoriesAsQueriable(string parentId);
        Task<CategoriesReturnDto> GetCategory(string catCode);
        Task<AnalyticsReturnObject> GetAnalytics(AnalyticsSearchParams searchParams);
    }
}
