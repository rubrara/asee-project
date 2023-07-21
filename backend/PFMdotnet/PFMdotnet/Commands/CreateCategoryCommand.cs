using CsvHelper.Configuration.Attributes;
using PFMdotnet.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.Transactions;

namespace PFMdotnet.Commands
{
    public class CreateCategoryCommand
    {
        [Required]
        [Name("code")]
        public string Code { get; set; }

        [Name("parent-code")]
        public string? ParentCode { get; set; }

        [Name("name")]
        public string Name { get; set; }

        public List<Transaction>? Transactions { get; set; }
    }
}
