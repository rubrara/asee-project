namespace PFMdotnet.Database.Entities
{
    public class TransactionSplit
    {
       
        public Guid Id { get; set; }
        public double Amount { get; set; }
        public string CatCode { get; set; }
        public string TransactionId { get; set; }
        public Transaction? Transaction { get; set; } = null;
    }
}
