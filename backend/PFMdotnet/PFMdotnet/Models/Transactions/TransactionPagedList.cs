using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PFMdotnet.Database.Enums;

namespace PFMdotnet.Models
{
    public class TransactionPagedList<T>
    {
        public int? TotalCount { get; set; } = null;

        public int? PageSize { get; set; } = null;

        public int? Page { get; set; } = null;  

        public int? TotalPages { get; set; } = null;

        public string? SortBy { get; set; } = null;

        public SortOrder? SortOrder { get; set; } = null;

        public DateOnly? StartDate { get; set; } = null;

        public DateOnly? EndDate { get; set; } = null;

        public List<KindEnum>? Kinds { get; set; } = null;

        public List<T>? Items { get; set; } = null;

        public string? Message { get; set; } = null;

        public List<string>? Errors { get; set; } = null;
    }

}
