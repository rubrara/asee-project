using AutoMapper;
using PFMdotnet.Commands;
using PFMdotnet.Database.Entities;
using PFMdotnet.Database.Repositories;
using PFMdotnet.Models;

namespace PFMdotnet.Services.Impl
{
    public class CategoryServiceImpl : ICategoryService
    {

        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryServiceImpl(ICategoryRepository repository, IMapper mapper)
        {
            _categoryRepository = repository;
            _mapper = mapper;
        }

        public async Task<List<Category>> CreateCategoryBulk(List<CreateCategoryCommand> commands)
        {
            var entities = _mapper.Map<List<CategoryEntity>>(commands);

            if (entities == null)
            {
                return null;
            }



            var result = await _categoryRepository.CreateBulk(entities);

            return _mapper.Map<List<Category>>(entities);
        }
    }
}
