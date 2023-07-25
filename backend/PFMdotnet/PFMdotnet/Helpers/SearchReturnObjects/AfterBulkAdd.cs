
namespace PFMdotnet.Models
{
    public class AfterBulkAdd<T>
    {

        public string? Message { get; set; } 


        public int? TotalRowsAdded { get; set; } = null;

        public int? TotalRowsUpdated { get; set; } = null;

        public string? Errors { get; set; } = null;

    }
}
