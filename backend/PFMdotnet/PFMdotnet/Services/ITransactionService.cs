using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PFMdotnet.Commands;
using PFMdotnet.Models;

namespace PFMdotnet.Services
{
    public interface ITransactionService
    {
        Task<Models.Transaction> CreateTransaction(CreateTransactionCommand command);
        Task<List<Models.Transaction>> CreateTransactionBulk(List<CreateTransactionCommand> commands);

        Task<PagedSortedList<Models.Transaction>> GetTransactions(int page = 1, int pageSize = 10);
        Task<Models.Transaction> GetProduct(string transactionCode);
    }
}
