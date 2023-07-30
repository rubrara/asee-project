using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using PFMdotnet.Database.Entities;
using PFMdotnet.Database.Enums;
using PFMdotnet.Helpers;
using PFMdotnet.Helpers.SearchReturnObjects.Categories;
using PFMdotnet.Models;

namespace PFMdotnet.Database.Repositories.Impl
{
    public class CategoryRepositoryImpl : ICategoryRepository
    {

        private readonly AppDbContext _dbContext;
        public CategoryRepositoryImpl(AppDbContext context)
        {
            _dbContext = context;
        }

        public async Task<AfterBulkAdd<Category>> CreateBulk(List<Category> categories, int chunkSize)
        {

            int total = categories.Count;
            int totalAdded = 0;
            int totalUpdated = 0;

            var uniqueIds = categories.Select(c => c.Code).ToList();

            var existingCategories = _dbContext.Categories
                .Where(c => uniqueIds.Contains(c.Code))
                .ToDictionary(t => t.Code);

            var newCategories = categories.Where(c => !existingCategories.ContainsKey(c.Code)).ToDictionary(c => c.Code);

            for (int i = 0; i < total; i += chunkSize)
            {

                var chunkNew = newCategories.Values.ToList().Skip(i).Take(chunkSize).ToList();
                var chunkExisting = existingCategories.Values.ToList().Skip(i).Take(chunkSize).ToList();

                if (chunkNew.Any())
                {
                    await _dbContext.Categories.AddRangeAsync(chunkNew);
                    totalAdded += chunkNew.Count();
                }

                if (chunkExisting.Any())
                {
                    _dbContext.Categories.UpdateRange(chunkExisting);
                    totalUpdated += chunkExisting.Count();
                }

                await _dbContext.SaveChangesAsync();
            }

            return new AfterBulkAdd<Category>
            {
                Message = "Uploading Categories form CSV file",
                TotalRowsAdded = totalAdded == 0 ? null : totalAdded,
                TotalRowsUpdated = totalUpdated == 0 ? null : totalUpdated,
            };
        }

        public async Task<Category?> FindByCode(string categoryCode)
        {
            return await _dbContext.Categories.FirstOrDefaultAsync(c => c.Code.Equals(categoryCode));

        }

        public async Task<List<Category>> GetAnalyticsAsync(string categoryCode)
        {

            var categoriesQuery = categoryCode == null ?
                 await _dbContext.Categories
                .Where(c => c.Transactions != null && c.Transactions.Any())
                .Include(c => c.Transactions).ToListAsync()
                :
                 await _dbContext.Categories
                .Where(c => (c.Transactions != null && c.Transactions.Any()) && (c.Code.Equals(categoryCode) || c.ParentCode.Equals(categoryCode)))
                .Include(c => c.Transactions).ToListAsync();

            return categoriesQuery;
        }

        public async Task<List<Category>> GetCategoriesAsync(FilterCategoriesParams searchParams)
        {

            IQueryable<Category> categories = searchParams.ParentId == null ?
                _dbContext.Categories.Where(c => string.IsNullOrEmpty(c.ParentCode)).Include(c => c.Transactions) :
                _dbContext.Categories.Where(c => c.ParentCode.Equals(searchParams.ParentId)).Include(c => c.Transactions);

            int page = searchParams.Page;
            int pageSize = searchParams.PageSize;

            if (!string.IsNullOrEmpty(searchParams.SortBy))
            {
                switch (searchParams.SortBy)
                {
                    case "code":
                        categories = string.Equals(searchParams.SortOrder, SortOrder.Asc.ToString(), StringComparison.OrdinalIgnoreCase) ?
                            categories.OrderBy(c => c.Code) : categories.OrderByDescending(c => c.Code);
                        break;

                    case "transactions":
                        categories = string.Equals(searchParams.SortOrder, SortOrder.Asc.ToString(), StringComparison.OrdinalIgnoreCase) ?
                            categories.OrderBy(c => c.Transactions.Count) : categories.OrderByDescending(c => c.Transactions.Count);
                        break;
                    default:
                        categories = string.Equals(searchParams.SortOrder, SortOrder.Asc.ToString(), StringComparison.OrdinalIgnoreCase) ?
                            categories.OrderBy(c => c.Code) : categories.OrderByDescending(c => c.Code);
                        break;
                }
            }
            else { categories = categories.OrderBy(c => c.Code); }

            
            return await categories.ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}