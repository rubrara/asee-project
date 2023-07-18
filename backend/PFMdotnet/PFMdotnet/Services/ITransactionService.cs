using PFMdotnet.Commands;

namespace PFMdotnet.Services
{
    public interface ITransactionService
    {
        Task<Models.Transaction> CreateTransaction(CreateTransactionCommand command);
        Task<List<Models.Transaction>> CreateTransactionBulk(List<CreateTransactionCommand> commands);
    }
}
