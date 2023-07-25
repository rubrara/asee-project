using PFMdotnet.Commands;
using PFMdotnet.Database.Entities;
using PFMdotnet.Models;

namespace PFMdotnet.Services
{
    public interface ITransactionService
    {
        Task<Transaction> CreateTransaction(CreateTransactionCommand command);
        Task<AfterBulkAdd<TransactionEntity>> CreateTransactionBulk(List<CreateTransactionCommand> commands);

        Task<ReturnDTO<Transaction>> GetTransaction(string transactionCode);

        Task<TransactionPagedList<TransactionEntity>> GetTransactionsAsQueriable(SearchTransactionParams searchParams);

        Task<ReturnDTO<Transaction>> AddCategoryToTransaction(string id, string catCode);

        Task<ReturnDTO<List<Transaction>>> SplitTransactionAsync(string transactionId, SplitByParams parameters);
        
    }
}
