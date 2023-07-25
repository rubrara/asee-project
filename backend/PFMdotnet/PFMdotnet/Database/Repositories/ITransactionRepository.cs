using PFMdotnet.Database.Entities;
using PFMdotnet.Helpers.SearchReturnObjects.Transactions;
using PFMdotnet.Models;

namespace PFMdotnet.Database.Repositories
{
    public interface ITransactionRepository
    {
        Task<Transaction> Create(Transaction transaction);
        Task<AfterBulkAdd<Transaction>> CreateBulk(List<Transaction> transactions, int chunkSize);
        Task<Transaction> Get(string Id);

        Task<TransactionPagedList<Transaction>> GetTransactionsAsQueryable(FilterTransactionsParams searchParams);

        Task<ReturnDTO<Transaction>> AddCategoryToTransaction(string id, string catCode);
        Task DeleteTransactionSplits(Transaction transaction);
        Task<bool> AddTransactionSplits(List<TransactionSplit> splits);
    }
}
