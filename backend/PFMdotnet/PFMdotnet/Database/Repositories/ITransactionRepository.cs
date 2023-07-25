using PFMdotnet.Database.Entities;
using PFMdotnet.Helpers.SearchReturnObjects.Transactions;
using PFMdotnet.Models;

namespace PFMdotnet.Database.Repositories
{
    public interface ITransactionRepository
    {
        Task<TransactionEntity> Create(TransactionEntity transaction);
        Task<AfterBulkAdd<TransactionEntity>> CreateBulk(List<TransactionEntity> transactions, int chunkSize);
        Task<TransactionEntity> Get(string Id);

        Task<TransactionPagedList<TransactionEntity>> GetTransactionsAsQueryable(FilterTransactionsParams searchParams);

        Task<ReturnDTO<TransactionEntity>> AddCategoryToTransaction(string id, string catCode);
    }
}
