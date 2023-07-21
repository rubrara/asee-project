using PFMdotnet.Database.Entities;
using PFMdotnet.Models;

namespace PFMdotnet.Database.Repositories
{
    public interface ITransactionRepository
    {
        Task<TransactionEntity> Create(TransactionEntity transaction);
        Task<List<TransactionEntity>> CreateBulk(List<TransactionEntity> transactions);
        Task<TransactionEntity> Get(string Id);

        Task<TransactionPagedList<TransactionEntity>> GetTransactionsAsQueryable(SearchParams searchParams);

        Task<TransactionEntity> AddCategoryToTransaction(string id, string catCode);
    }
}
