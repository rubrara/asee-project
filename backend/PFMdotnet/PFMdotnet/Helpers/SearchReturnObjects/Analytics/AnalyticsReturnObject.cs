using PFMdotnet.Models.Analytics;

namespace PFMdotnet.Helpers.SearchReturnObjects.Analytics
{
    public class AnalyticsReturnObject
    {
        public string? Message { get; set; }

        public List<AnalyticsDto>? Groups { get; set; } = null;

        public AnalyticsGroup? top_level { get; set; } = null;

        public AnalyticsGroup? sub_category { get; set; } = null;

        public AnalyticsDto? Analytics { get; set; } = null;
        public List<string>? Errors { get; set; } = null;
        public string? Result { get; set; } = null;

    }
}
