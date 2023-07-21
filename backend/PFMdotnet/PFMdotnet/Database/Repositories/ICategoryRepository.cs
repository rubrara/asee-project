using PFMdotnet.Database.Entities;

namespace PFMdotnet.Database.Repositories
{
    public interface ICategoryRepository
    {

        Task<List<CategoryEntity>> CreateBulk(List<CategoryEntity> categories);

        // Task<List<CategoryEntity>> GetAll();

        Task<CategoryEntity> FindByCode(string categoryCode);



    }
}
