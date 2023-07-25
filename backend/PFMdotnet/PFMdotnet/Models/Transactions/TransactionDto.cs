using PFMdotnet.Database.Enums;

namespace PFMdotnet.Models
{
    public class TransactionDto
    {
        public string Id { get; set; }
        public string BeneficiaryName { get; set; }
        public DateOnly Date { get; set; }
        public DirectionEnum Direction { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public CurrencyEnum Currency { get; set; }
        public int? Mcc { get; set; }    
        public KindEnum Kind { get; set; }

        public string? CatCode { get; set; }


    }
}
