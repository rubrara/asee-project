using PFMdotnet.Commands;
using PFMdotnet.Models;

namespace PFMdotnet.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> CreateCategoryBulk(List<CreateCategoryCommand> commands);
    }
}
