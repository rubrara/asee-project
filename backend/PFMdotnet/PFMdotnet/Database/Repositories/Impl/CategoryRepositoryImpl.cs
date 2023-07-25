using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using PFMdotnet.Database.Entities;
using PFMdotnet.Database.Enums;
using PFMdotnet.Helpers;
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

        public async Task<AfterBulkAdd<CategoryEntity>> CreateBulk(List<CategoryEntity> categories, int chunkSize)
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

            return new AfterBulkAdd<CategoryEntity>
            {
                Message = "Uploading Categories form CSV file",
                TotalRowsAdded = totalAdded == 0 ? null : totalAdded,
                TotalRowsUpdated = totalUpdated == 0 ? null : totalUpdated,
            };
        }

        public async Task<CategoryEntity?> FindByCode(string categoryCode)
        {
            return await _dbContext.Categories.FirstOrDefaultAsync(c => c.Code.Equals(categoryCode));
        }

        public async Task<List<CategoryEntity>> GetAnalyticsAsync(string categoryCode)
        {

            IQueryable<CategoryEntity> categoriesQuery = _dbContext.Categories
                .Where(c => c.Transactions != null && c.Transactions.Any())
                .Include(c => c.Transactions);

            if (categoryCode is not null)
            {
                categoriesQuery = categoriesQuery
                    .Where(c => c.Code.Equals(categoryCode) || c.ParentCode.Equals(categoryCode));
            }

            var categories = await categoriesQuery.ToListAsync();

            return categories;

        }

        public async Task<List<CategoryEntity>> GetCategoriesAsync(string parentId)
        {

            var categories = parentId == null ?
                await _dbContext.Categories.Where(c => string.IsNullOrEmpty(c.ParentCode)).ToListAsync() :
                await _dbContext.Categories.Where(c => c.ParentCode.Equals(parentId)).ToListAsync();

            return categories;
        }
    }
}