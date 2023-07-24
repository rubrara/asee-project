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

        public async Task<AfterBulkAdd<CategoryEntity>> CreateBulk(List<CategoryEntity> categories)
        {

            int chunkSize = 500;
            int total = categories.Count;
            var chunks = ListToChunks.ChunkList(categories, chunkSize);

            try
            {
                foreach (var chunk in chunks)
                {
                    await _dbContext.Categories.AddRangeAsync(chunk);
                    await _dbContext.SaveChangesAsync();
                }


                return new AfterBulkAdd<CategoryEntity>
                {
                    toAdd = "Categories",
                    totalRowsAdded = total,
                    rowExample = categories.ElementAt(0)
                };
            }
            catch (Exception)
            {
                return new AfterBulkAdd<CategoryEntity>
                {
                    toAdd = "Categories",
                    totalRowsAdded = 0,
                    Error = "There was an error trying to add the categories!"
                };
            }

        }

        public async Task<CategoryEntity> FindByCode(string categoryCode)
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
    }
}