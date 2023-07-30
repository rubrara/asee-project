using PFMdotnet.Commands;
using PFMdotnet.Database.Entities;
using PFMdotnet.Helpers.SearchReturnObjects.Analytics;
using PFMdotnet.Helpers.SearchReturnObjects.Categories;
using PFMdotnet.Models;
using PFMdotnet.Models.Categories;

namespace PFMdotnet.Services
{
    public interface ICategoryService
    {
        Task<AfterBulkAdd<Category>> CreateCategoryBulk(List<CreateCategoryCommand> commands);

        Task<CategoriesPagedList<CategoryDto>> GetCategoriesAsQueriable(SearchCategoriesParams searchParams);
        Task<CategoriesReturnDto> GetCategory(string catCode);
        Task<AnalyticsReturnObject> GetAnalytics(AnalyticsSearchParams searchParams);
    }
}
