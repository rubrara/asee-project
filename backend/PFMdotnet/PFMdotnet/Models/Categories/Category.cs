using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations;

namespace PFMdotnet.Models
{
    public class Category
    {
        public string Code { get; set; }

        public string? ParentCode { get; set; }

        public string Name { get; set; }

        //public List<Transaction>? Transactions { get; set; }
    }
}
