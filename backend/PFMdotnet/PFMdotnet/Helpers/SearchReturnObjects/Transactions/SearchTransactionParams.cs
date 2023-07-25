using PFMdotnet.Database.Enums;

namespace PFMdotnet.Models
{
    public class SearchTransactionParams
    {

        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
        public string? Kinds { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }

    }
}
