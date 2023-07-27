using Microsoft.EntityFrameworkCore;
using PFMdotnet.Database.Entities;
using PFMdotnet.Helpers.SearchReturnObjects.Transactions;
using PFMdotnet.Models;
using PFMdotnet.Models.Rules;

namespace PFMdotnet.Database.Repositories
{
    public interface ITransactionRepository
    {
        Task<Transaction> Create(Transaction transaction);
        Task<AfterBulkAdd<Transaction>> CreateBulk(List<Transaction> transactions, int chunkSize);
        Task<Transaction> Get(string Id);
        Task<TransactionDto> GetAsDto(string Id);
        Task<TransactionPagedList<TransactionDto>> GetTransactionsAsQueryable(FilterTransactionsParams searchParams);
        Task<ReturnDTO<Transaction>> AddCategoryToTransaction(string id, string catCode);
        Task DeleteTransactionSplits(Transaction transaction);
        Task<bool> AddTransactionSplits(List<TransactionSplit> splits);
        Task<int> AutoCategorize(List<CategorizationRule> rules);
        Task SaveChangesAsync();
    }
}
