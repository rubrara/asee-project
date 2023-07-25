namespace PFMdotnet.Database.Entities
{
    public class Category
    {
        public string Code { get; set; }

        public string? ParentCode { get; set; }

        public string Name { get; set; }

        public List<Transaction>? Transactions { get; set; }
    }
}
