using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using PFMdotnet.Commands;
using PFMdotnet.Database.Entities;
using PFMdotnet.Database.Enums;
using PFMdotnet.Database.Repositories;
using PFMdotnet.Helpers.SearchReturnObjects.Analytics;
using PFMdotnet.Helpers.SearchReturnObjects.Categories;
using PFMdotnet.Models;
using PFMdotnet.Models.Analytics;
using System.Reflection.Emit;

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

        public async Task<AfterBulkAdd<Category>> CreateCategoryBulk(List<CreateCategoryCommand> commands)
        {
            var entities = _mapper.Map<List<Category>>(commands);

            if (entities == null)
            {
                return null;
            }


            int chunkSize = 200;
            var result = await _categoryRepository.CreateBulk(entities, chunkSize);

            return result;
        }


        public async Task<AnalyticsReturnObject> GetAnalytics(AnalyticsSearchParams searchParams)
        {
            var categoryCode = searchParams.Catcode;
            var categories = await _categoryRepository.GetAnalyticsAsync(categoryCode);

            List<string> errors = new();
            var result = new AnalyticsReturnObject()
            {
                Message = "Retrieve spending analytics by category or by subcategories witin category"
            };

            DateOnly startDate;
            DateOnly endDate;

            if (string.IsNullOrEmpty(searchParams.StartDate))
            {
                startDate = DateOnly.MinValue;

            }
            else if (!DateOnly.TryParse(searchParams.StartDate, out startDate))
            {
                errors.Add(string.Format("Invalid date format for the StartDate: '{0}'. Please provide a valid date in the format 'YYYY-MM-DD'.", searchParams.StartDate));

            }

            if (string.IsNullOrEmpty(searchParams.EndDate))
            {
                endDate = DateOnly.MaxValue;

            }
            else if (!DateOnly.TryParse(searchParams.EndDate, out endDate))
            {
                errors.Add(string.Format("Invalid date format for the EndDate: '{0}'. Please provide a valid date in the format 'YYYY-MM-DD'.", searchParams.EndDate));
            }

            List<DirectionEnum> directions = new();
            DirectionEnum direction = DirectionEnum.d;

            if (string.IsNullOrEmpty(searchParams.Direction))
            {
                directions = Enum.GetValues(typeof(DirectionEnum)).Cast<DirectionEnum>().ToList();

            } else if (!Enum.TryParse(searchParams.Direction, true, out direction))
            {
                errors.Add(string.Format("'{0}' is NOT part of the Direction enum.", searchParams.Direction));
            }

            

            if (categories.Count == 0)
            {
                errors.Add("There are NOT any Transactions on that Category Code");
            }


            if (errors.Any())
            {
                result.Errors = errors;

                return result;
            }

            directions.Add(direction);


            do
            {
                if (categoryCode != null)
                {

                    if (_categoryRepository.FindByCode(categoryCode) == null)
                    {
                        errors.Add(string.Format("The Category with code: '{0}' doesn't exist", categoryCode));
                        break;
                    }

                    var tmpCats = categories.Where(c => c.ParentCode.Equals(categoryCode)).ToList();


                    double amountToAdd = 0;
                    int countToAdd = 0;

                    

                    if (tmpCats.Any())
                    {
                        foreach (var cat in tmpCats)
                        {
                            var transactions = cat.Transactions
                                .Where(t => directions.Contains(t.Direction) && (t.Date > startDate && t.Date < endDate))
                                .ToList();

                            amountToAdd += transactions.Sum(t => t.Amount);
                            countToAdd = transactions.Count();
                        }
                    }

                    var category = categories.Find(c => c.Code.Equals(categoryCode));

                    

                    double amountFromCat = 0;
                    int countFromCat = 0;

                    if (category != null)
                    {
                        var categoryTransactions = category.Transactions
                                .Where(t => directions.Contains(t.Direction) && (t.Date > startDate && t.Date < endDate))
                                .ToList();

                        amountFromCat = categoryTransactions.Sum(t => t.Amount);
                        countFromCat = categoryTransactions.Count();
                    }
                    

                    

                    if(countFromCat + countToAdd == 0)
                    {
                        result.Result = "The are not any Transactions for the category code given the filters";
                        return result;
                    }


                    result.Analytics = new AnalyticsDto()
                    {
                        CatCode = categoryCode,
                        Amount = Math.Round(amountFromCat + amountToAdd, 2),
                        Count = countFromCat + countToAdd
                    };

                    return result;
                }

            } while (false);            

           


            List<string> childCategories = new();

            var groupedAnalytics = new Dictionary<string, AnalyticsDto>();

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
                    childCategories.Add(cat.Code);

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

                result.Result = "The database doesn't contain the spendings given the filters";

                return result;
            }

            var children = new AnalyticsGroup();
            var parents = new AnalyticsGroup();

            foreach (var analytics in groupedAnalytics)
            {
                analytics.Value.Amount = Math.Round(analytics.Value.Amount, 2);

                if (childCategories.Contains(analytics.Key))
                {
                    children.Groups.Add(analytics.Value);
                } else
                {
                    parents.Groups.Add(analytics.Value);
                }
            }

            result.top_level = parents.Groups.Any() ? parents : null;
            result.sub_category = children.Groups.Any() ? children : null;

            return result;
        }

        public async Task<CategoriesReturnDto> GetCategoriesAsQueriable(string parentId)
        {
            // _mapper.Map<Category>(category)
            var categories = _mapper.Map<List<CategoryDto>>( await _categoryRepository.GetCategoriesAsync(parentId));
            var res = new CategoriesReturnDto()
            {
                Message = "Getting the top-level categories from database"
            };

            if(parentId != null)
            {

                res.Message = string.Format("Getting the sub-categories for category: {0}", parentId);

                if (await _categoryRepository.FindByCode(parentId) == null)
                {
                    res.Errors = new()
                    {
                        string.Format("'{0}' doesn't exist in the database", parentId)
                    };

                    return res;
                }

                if(!categories.Any())
                {
                    res.Result = string.Format("'{0}' doesn't have any sub-categories", parentId);

                    return res;
                }

                res.Categories = new();

                foreach (var category in categories)
                {
                    res.Categories.Add(category);
                }

                return res;
            }
            else
            {
                if(!categories.Any()) {
                    res.Result = "There are not any top-level categories";
                    return res;
                }

                res.Categories = new();

                foreach (var category in categories)
                {
                    res.Categories.Add(category);
                }

                return res;
            }
            
        }

        public async Task<CategoriesReturnDto> GetCategory(string catCode)
        {

            var category = await _categoryRepository.FindByCode(catCode);

            var res = new CategoriesReturnDto()
            {
                Message = string.Format("Getting Category by code: '{0}'", catCode)
            };

            if (string.IsNullOrEmpty(catCode))
            {

                res.Errors = new()
                {
                    "The category code MUST NOT be null or empty!"
                };

                return res;

            }

            if (category == null)
            {
                res.Errors = new()
                {
                    string.Format("There is NOT any categories with code: '{0}'", catCode)
                };

                return res;
            }

            res.Category = _mapper.Map<CategoryDto>(category);

            return res;
        }
    }
}
