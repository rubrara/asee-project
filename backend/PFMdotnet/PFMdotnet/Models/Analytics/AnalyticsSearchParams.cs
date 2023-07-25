using CsvHelper.Configuration.Attributes;
using PFMdotnet.Database.Enums;

namespace PFMdotnet.Models.Analytics
{
    public class AnalyticsSearchParams
    {

        public string? Catcode { get; set; } = null;
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? Direction { get; set; } = null;

    }
}
