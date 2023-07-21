using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PFMdotnet.Commands;
using PFMdotnet.Database.Entities;
using PFMdotnet.Models;

namespace PFMdotnet.Services
{
    public interface ITransactionService
    {
        Task<Transaction> CreateTransaction(CreateTransactionCommand command);
        Task<List<Transaction>> CreateTransactionBulk(List<CreateTransactionCommand> commands);

        Task<Transaction> GetTransaction(string transactionCode);

        Task<TransactionPagedList<TransactionEntity>> GetTransactionsAsQueriable(SearchParams searchParams);

        Task<Transaction> AddCategoryToTransaction(string id, string catCode);
    }
}
