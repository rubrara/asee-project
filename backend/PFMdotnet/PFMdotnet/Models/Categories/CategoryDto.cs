
namespace PFMdotnet.Models
{
    public class CategoryDto
    {
        public string Code { get; set; }

        public string? ParentCode { get; set; }

        public string Name { get; set; }

        //public List<Transaction>? Transactions { get; set; }
    }
}
