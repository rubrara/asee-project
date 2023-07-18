using CsvHelper.Configuration.Attributes;
using PFMdotnet.Database.Entities;
using PFMdotnet.Database.Enums;
using System.ComponentModel.DataAnnotations;

namespace PFMdotnet.Commands
{
    public class CreateTransactionCommand
    {
        [Required]
        [Name("id")]
        public string Id { get; set; }
        [Name("beneficiary-name")]
        public string BeneficiaryName { get; set; }
        [Name("date")]
        public DateOnly Date { get; set; }
        
        [Required]
        [Name("direction")]
        public DirectionEnum Direction { get; set; }
        [Name("amount")]
        public double Amount { get; set; }
        [Name("description")]
        public string Description { get; set; }
        [Required]
        [Name("currency")]
        public CurrencyEnum Currency { get; set; }
        [Name("mcc")]
        public int? Mcc { get; set; }
        [Required]
        [Name("kind")]
        public KindEnum Kind { get; set; }
    }
}
