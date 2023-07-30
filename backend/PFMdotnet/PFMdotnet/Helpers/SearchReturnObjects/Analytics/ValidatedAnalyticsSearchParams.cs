using PFMdotnet.Database.Enums;

namespace PFMdotnet.Helpers.SearchReturnObjects.Analytics
{
    public class ValidatedAnalyticsSearchParams
    {
        public string? Catcode { get; set; } = null;
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public List<DirectionEnum>? Directions { get; set; } = null;
        public List<string>? Errors { get; set; } = null;
    }
}
