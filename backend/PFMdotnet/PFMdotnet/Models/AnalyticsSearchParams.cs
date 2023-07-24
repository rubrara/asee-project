using CsvHelper.Configuration.Attributes;
using PFMdotnet.Database.Enums;

namespace PFMdotnet.Models
{
    public class AnalyticsSearchParams
    {

        public string? Catcode { get; set; } = null;
        public string? StartDate { get; set; }       
        public string? EndDate { get; set; } 
        public DirectionEnum? Direction { get; set; } = null;

    }
}
