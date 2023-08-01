using PFMdotnet.Database.Enums;
using PFMdotnet.Helpers.SearchReturnObjects.Analytics;
using PFMdotnet.Helpers.SearchReturnObjects.Categories;
using PFMdotnet.Helpers.SearchReturnObjects.Transactions;
using PFMdotnet.Models;

namespace PFMdotnet.Helpers.Validation
{
    public class Validate
    {

        public static ValidatedAnalyticsSearchParams ValidateAnalyticParams(AnalyticsSearchParams searchParams)
        {
            var result = new ValidatedAnalyticsSearchParams();
            List<string> errors = new();

            string? CatCode = searchParams.Catcode;
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

            }
            else if (!Enum.TryParse(searchParams.Direction, true, out direction))
            {
                errors.Add(string.Format("'{0}' is NOT part of the Direction enum.", searchParams.Direction));
            }

            directions.Add(direction);

            if (errors.Any())
            {
                result.Errors = errors;

                return result;
            }

            result.EndDate = endDate;
            result.StartDate = startDate;
            result.Catcode = CatCode;
            result.Directions = directions;

            return result;
        }

        public static FilterTransactionsParams ValidateTransactionFilterParams(SearchTransactionParams searchParams)
        {
            var result = new FilterTransactionsParams();
            List<KindEnum>? kinds = new();
            List<string> errors = new();

            if (!string.IsNullOrEmpty(searchParams.Kinds))
            {
                char[] delimiters = { ' ', ',' };
                string[] kindsArray = searchParams.Kinds.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                foreach (string kind in kindsArray)
                {
                    try
                    {
                        kinds.Add(Enum.Parse<KindEnum>(kind));
                    }
                    catch (Exception)
                    {
                        errors.Add(string.Format("'{0}' is NOT a valid transaction kind", kind));
                    }
                }
            }
            else
            {
                kinds = Enum.GetValues(typeof(KindEnum)).Cast<KindEnum>().ToList();
            }

            int page = searchParams.Page > 1 ? (int)searchParams.Page : 1;
            int pageSize = (searchParams.PageSize < 1 || searchParams.PageSize == null) ? 5 : searchParams.PageSize > 50 ? 50 : (int)searchParams.PageSize;

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

            SortOrder sortOrder;

            /*if (string.IsNullOrEmpty(searchParams.SortOrder) || !(Enum.IsDefined(typeof(SortOrder), searchParams.SortOrder)))
            {
                sortOrder = SortOrder.Asc;
            }
            else*/ if (!Enum.TryParse(searchParams.SortOrder, true, out sortOrder))
            {
                sortOrder=SortOrder.Asc;
            }

            if (errors.Any())
            {
                result.Errors = errors;
                return result;
            }

            result.StartDate = startDate; 
            result.EndDate = endDate;
            result.Page = (int)page;
            result.PageSize = (int)pageSize;
            result.Kinds = kinds;
            result.SortOrder = sortOrder;
            result.SortBy = searchParams.SortBy != null ? searchParams.SortBy : "Id";

            return result;
        }

        public static FilterCategoriesParams ValidateCategoriesFilterParams(SearchCategoriesParams searchParams)
        {
            var result = new FilterCategoriesParams();

            int page = searchParams.Page > 1 ? (int)searchParams.Page : 1;
            int pageSize = (searchParams.PageSize == null || searchParams.PageSize < 1) ? 5 : searchParams.PageSize > 50 ? 50 : (int)searchParams.PageSize;

            if (!string.IsNullOrEmpty(searchParams.SortOrder))
            {
                result.SortOrder = searchParams.SortOrder.ToLower();
            }
            if (!string.IsNullOrEmpty(searchParams.SortBy))
            {
                result.SortBy = searchParams.SortBy.ToLower();  
            }

            result.Page = page;
            result.PageSize = pageSize;
            result.ParentId = searchParams.ParentId;

            return result;
        }
    }
}
