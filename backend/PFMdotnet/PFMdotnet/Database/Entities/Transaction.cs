using CsvHelper.Configuration.Attributes;
using PFMdotnet.Database.Enums;

namespace PFMdotnet.Database.Entities
{
    public class Transaction
    {
        public string Id { get; set; }
        [Name("beneficiary-name")]
        public string? BeneficiaryName { get; set; }
        public DateOnly Date { get; set; }
        public DirectionEnum Direction { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public CurrencyEnum Currency { get; set; }
        public int? Mcc { get; set; }
        public KindEnum Kind { get; set; }
        public Category? Category { get; set; }
        public string? CatCode { get; set; }
        public List<TransactionSplit>? Splits { get; set; } = null;
    }
}
