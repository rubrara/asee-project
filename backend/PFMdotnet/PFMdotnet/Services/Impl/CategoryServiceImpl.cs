using AutoMapper;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using PFMdotnet.Commands;
using PFMdotnet.Database.Entities;
using PFMdotnet.Database.Enums;
using PFMdotnet.Database.Repositories;
using PFMdotnet.Helpers.SearchReturnObjects.Analytics;
using PFMdotnet.Helpers.SearchReturnObjects.Categories;
using PFMdotnet.Helpers.Validation;
using PFMdotnet.Models;
using PFMdotnet.Models.Analytics;
using PFMdotnet.Models.Categories;
using System.Data;
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
            var result = new AnalyticsReturnObject()
            {
                Message = "Retrieve spending analytics by category or by subcategories witin category"
            };

            if (categoryCode != null)
            {
                if (await _categoryRepository.FindByCode(categoryCode) == null)
                {
                    result.Result = string.Format("The Category with code: '{0}' doesn't exist", categoryCode);
                    return result;
                }
            }

            var parameters = Validate.ValidateAnalyticParams(searchParams);

            if (parameters.Errors != null)
            {
                result.Errors = parameters.Errors;

                return result;
            }

            var categories = await _categoryRepository.GetAnalyticsAsync(categoryCode);

            if (categoryCode != null)
            {
                double amountToAdd = 0;
                int countToAdd = 0;      

                foreach (var cat in categories)
                {
                    var transactions = cat.Transactions
                        .Where(t => parameters.Directions.Contains(t.Direction) && (t.Date > parameters.StartDate && t.Date < parameters.EndDate))
                        .ToList();

                    amountToAdd += transactions.Sum(t => t.Amount);
                    countToAdd += transactions.Count();
                }                    

                if(countToAdd == 0)
                {
                    result.Result = "The are not any Transactions for the category code given the filters";
                    return result;
                }

                result.Groups = new List<AnalyticsDto>()
                {
                    new AnalyticsDto()
                    {
                        CatCode = categoryCode,
                        Amount = Math.Round(amountToAdd, 2),
                        Count = countToAdd
                    }
                };

                return result;
            }

            result.Groups = new List<AnalyticsDto>();

            foreach (var cat in categories)
            {
                var transactions = cat.Transactions
                    .Where(t => parameters.Directions.Contains(t.Direction) && (t.Date > parameters.StartDate && t.Date < parameters.EndDate))
                    .ToList();

                var amount = transactions.Sum(t => t.Amount);
                var count = transactions.Count;

                if (count == 0) continue;

                result.Groups.Add(new AnalyticsDto
                {
                    CatCode = cat.Code,
                    Amount = Math.Round(amount, 2),
                    Count = count
                });              
            }

            return result;
        }

        public async Task<CategoriesPagedList<CategoryDto>> GetCategoriesAsQueriable(SearchCategoriesParams searchParams)
        {
    
            var searchCategoriesParams = Validate.ValidateCategoriesFilterParams(searchParams);
            string parentId = searchCategoriesParams.ParentId;

            var categories = _mapper.Map<List<CategoryDto>>( await _categoryRepository.GetCategoriesAsync(searchCategoriesParams));

            var res = new CategoriesPagedList<CategoryDto>()
            {
                Message = "Getting the top-level categories from database"
            };

            if(parentId != null)
            {

                res.Message = string.Format("Getting the sub-categories for category: {0}", parentId);

                if (await _categoryRepository.FindByCode(parentId) == null)
                {
                    res.Message = string.Format("'{0}' doesn't exist in the database", parentId);
                    return res;
                }
                /*if(!categories.Any())
                {
                    res.Message = string.Format("'{0}' doesn't have any sub-categories", parentId);
                    return res;
                }*/

            }
            else if(!categories.Any()) {
                res.Message = "There are not any top-level categories";
                return res;
            }

            int page = searchCategoriesParams.Page;
            int pageSize = searchCategoriesParams.PageSize;
            int totalCount = categories.Count();
            int totalPages = (int) Math.Ceiling(totalCount * 1.0 / pageSize);

            categories = categories.Skip((page - 1) * pageSize)
                               .Take(pageSize).ToList();

            res.Page = page;
            res.PageSize = pageSize;
            res.TotalCount = totalCount;
            res.TotalPages = totalPages;
            res.Items = new();

            foreach (var category in categories)
            {
                res.Items.Add(category);
            }

            return res;

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
