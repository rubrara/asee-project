using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PFMdotnet.Database.Enums;

namespace PFMdotnet.Models
{
    public class SearchParams
    {

        public int page = 1;
        public int pageSize = 10;
        public string? sortBy;
        public SortOrder sortOrder = SortOrder.Asc;
        public List<KindEnum>? kinds;
        public DateOnly startDate;
        public DateOnly endDate;

    }
}
