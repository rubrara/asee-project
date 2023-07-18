using Microsoft.EntityFrameworkCore;
using PFMdotnet.Database.Entities;

namespace PFMdotnet.Database.Repositories.Impl
{
    public class TransactionRepositoryImpl : ITransactionRepository
    {

        private readonly TransactionDbContext _dbContext;
        public TransactionRepositoryImpl(TransactionDbContext context) {
            _dbContext = context;
        }

        public async Task<TransactionEntity> Create(TransactionEntity transaction)
        {
            _dbContext.Transactions.Add(transaction);

            await _dbContext.SaveChangesAsync();

            return transaction;
        }

        public async Task<List<TransactionEntity>> CreateBulk(List<TransactionEntity> transactions)
        {
            await _dbContext.Transactions.AddRangeAsync(transactions);

            await _dbContext.SaveChangesAsync();

            return transactions;
        }

        public async Task<TransactionEntity> Get(string Id)
        {
            return await _dbContext.Transactions.FirstOrDefaultAsync(p => p.Id == Id);
        }
    }
}
