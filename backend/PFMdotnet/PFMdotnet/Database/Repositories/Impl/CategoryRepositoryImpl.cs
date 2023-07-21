using Microsoft.EntityFrameworkCore;
using PFMdotnet.Database.Entities;

namespace PFMdotnet.Database.Repositories.Impl
{
    public class CategoryRepositoryImpl : ICategoryRepository
    {

        private readonly AppDbContext _dbContext;
        public CategoryRepositoryImpl(AppDbContext context)
        {
            _dbContext = context;
        }

        public async Task<List<CategoryEntity>> CreateBulk(List<CategoryEntity> categories)
        {
            
            await _dbContext.Categories.AddRangeAsync(categories);

            await _dbContext.SaveChangesAsync();

            return categories;
            
        }

        public async Task<CategoryEntity> FindByCode(string categoryCode)
        {

            if (string.IsNullOrWhiteSpace(categoryCode))
            {
                throw new ArgumentNullException(nameof(categoryCode));
            }

            return await _dbContext.Categories.FirstOrDefaultAsync(c => c.Code.Equals(categoryCode));
        }
    }
}
