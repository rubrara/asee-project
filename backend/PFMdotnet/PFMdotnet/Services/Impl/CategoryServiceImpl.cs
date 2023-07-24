using AutoMapper;
using Newtonsoft.Json.Linq;
using PFMdotnet.Commands;
using PFMdotnet.Database.Entities;
using PFMdotnet.Database.Enums;
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

        public async Task<AfterBulkAdd<CategoryEntity>> CreateCategoryBulk(List<CreateCategoryCommand> commands)
        {
            var entities = _mapper.Map<List<CategoryEntity>>(commands);

            if (entities == null)
            {
                return null;
            }



            var result = await _categoryRepository.CreateBulk(entities);

            return result;
        }


        // TODO 
        public async Task<ReturnDTO<AnalyticsGroup>> GetAnalytics(AnalyticsSearchParams searchParams)
        {
            var categoryCode = searchParams.Catcode;

            var categories = await _categoryRepository.GetAnalyticsAsync(categoryCode);

            string process = "Getting Analytics view of Transactions by Category";
            var returnDto = new ReturnDTO<AnalyticsGroup> { Process = process };

            string error = "There are NOT any Transactions on that Category Code";

            if (categories.Count == 0)
            {
                returnDto.Errors = new() { error };
                return returnDto;
            }

            var startDate = string.IsNullOrEmpty(searchParams.StartDate) ? DateOnly.MinValue : DateOnly.Parse(searchParams.StartDate);
            var endDate = string.IsNullOrEmpty(searchParams.EndDate) ? DateOnly.MaxValue : DateOnly.Parse(searchParams.EndDate);
            List<DirectionEnum> directions = searchParams.Direction != null
                ? new() { searchParams.Direction.Value }
                : Enum.GetValues<DirectionEnum>().ToList();

            var groupedAnalytics = new Dictionary<string, Analytics>();

            foreach (var cat in categories)
            {
                var transactions = cat.Transactions
                    .Where(t => directions.Contains(t.Direction) && (t.Date > startDate && t.Date < endDate))
                    .ToList();

                var amount = transactions.Sum(t => t.Amount);
                var count = transactions.Count;

                if (count == 0) continue;

                if (!string.IsNullOrEmpty(cat.ParentCode))
                {
                    if (!groupedAnalytics.TryGetValue(cat.ParentCode, out var parentAnalytics))
                    {
                        parentAnalytics = new()
                        {
                            CatCode = cat.ParentCode,
                            Amount = 0,
                            Count = 0
                        };
                        groupedAnalytics[cat.ParentCode] = parentAnalytics;
                    }

                    parentAnalytics.Amount += amount;
                    parentAnalytics.Count += count;
                }

                if (!groupedAnalytics.ContainsKey(cat.Code))
                {
                    groupedAnalytics[cat.Code] = new()
                    {
                        CatCode = cat.Code,
                        Amount = amount,
                        Count = count
                    };
                }
                else
                {
                    var item = groupedAnalytics[cat.Code];
                    item.Count += count;
                    item.Amount += amount;
                }
            }

            if (groupedAnalytics.Count == 0)
            {
                returnDto.Errors = new() { error };

                return returnDto;
            }


            var value = new AnalyticsGroup();
            
            value.Group = groupedAnalytics.Values.ToList();

            returnDto.Value = value;

            return returnDto;
        }

        public async Task<Category> GetCategory(string catCode)
        {
            var res = await _categoryRepository.FindByCode(catCode);

            return _mapper.Map<Category>(res);
        }
    }
}
