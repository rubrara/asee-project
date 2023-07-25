using PFMdotnet.Database.Enums;
using PFMdotnet.Models;

namespace PFMdotnet.Helpers.SearchReturnObjects.Transactions
{
    public class FilterTransactionsParams
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string? SortBy { get; set; }
        public SortOrder? SortOrder { get; set; }
        public List<KindEnum>? Kinds { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
