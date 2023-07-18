using PFMdotnet.Database.Entities;

namespace PFMdotnet.Database.Repositories
{
    public interface ITransactionRepository
    {
        Task<TransactionEntity> Create(TransactionEntity transaction);
        Task<List<TransactionEntity>> CreateBulk(List<TransactionEntity> transactions);
        Task<TransactionEntity> Get(string Id);
    }
}
