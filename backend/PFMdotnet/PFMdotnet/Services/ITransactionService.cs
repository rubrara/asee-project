using PFMdotnet.Commands;
using PFMdotnet.Database.Entities;
using PFMdotnet.Models;

namespace PFMdotnet.Services
{
    public interface ITransactionService
    {
        Task<TransactionDto> CreateTransaction(CreateTransactionCommand command);
        Task<AfterBulkAdd<Transaction>> CreateTransactionBulk(List<CreateTransactionCommand> commands);

        Task<ReturnDTO<TransactionDto>> GetTransaction(string transactionCode);

        Task<TransactionPagedList<Transaction>> GetTransactionsAsQueriable(SearchTransactionParams searchParams);

        Task<ReturnDTO<TransactionDto>> AddCategoryToTransaction(string id, string catCode);

        Task<ReturnDTO<List<TransactionDto>>> SplitTransactionAsync(string transactionId, SplitByParams parameters);
        
    }
}
