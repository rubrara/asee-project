
namespace PFMdotnet.Models
{
    public class AfterBulkAdd<T>
    {

        public string toAdd { get; set; }


        public int totalRowsAdded { get; set; }

        public string? Error { get; set; }
        public T rowExample { get; set; }

    }
}
