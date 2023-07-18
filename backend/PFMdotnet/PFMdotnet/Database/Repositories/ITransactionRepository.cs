using PFMdotnet.Database.Entities;
using PFMdotnet.Models;

namespace PFMdotnet.Database.Repositories
{
    public interface ITransactionRepository
    {
        Task<TransactionEntity> Create(TransactionEntity transaction);
        Task<List<TransactionEntity>> CreateBulk(List<TransactionEntity> transactions);
        Task<TransactionEntity> Get(string Id);

        Task<PagedSortedList<TransactionEntity>> List(int page = 1, int pageSize = 9);
    }
}
