using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PFMdotnet.Database.Enums;

namespace PFMdotnet.Models
{
    public class TransactionPagedList<T>
    {
        public int TotalCount { get; set; }

        public int PageSize { get; set; }

        public int Page { get; set; }

        public int TotalPages { get; set; }

        public string SortBy { get; set; }

        public SortOrder SortOrder { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public List<KindEnum> Kinds { get; set; }

        public List<T> Items { get; set; }
    }

}
