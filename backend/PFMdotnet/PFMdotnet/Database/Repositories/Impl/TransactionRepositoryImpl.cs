using Microsoft.EntityFrameworkCore;
using PFMdotnet.Database.Entities;
using PFMdotnet.Models;

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

        public async Task<PagedSortedList<TransactionEntity>> List(int page = 1, int pageSize = 9)
        {
            var query = _dbContext.Transactions.AsQueryable();

            var totalCount = query.Count();

            var totalPages = (int)Math.Ceiling(totalCount * 1.0 / pageSize);

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            var items = await query.ToListAsync();

            return new PagedSortedList<TransactionEntity>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Items = items
            };
        }
    }
 }

