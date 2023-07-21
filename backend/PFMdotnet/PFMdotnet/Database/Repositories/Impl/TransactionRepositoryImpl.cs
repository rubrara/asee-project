using Microsoft.EntityFrameworkCore;
using PFMdotnet.Database.Entities;
using PFMdotnet.Models;

namespace PFMdotnet.Database.Repositories.Impl
{
    public class TransactionRepositoryImpl : ITransactionRepository
    {

        private readonly AppDbContext _dbContext;
        private readonly ICategoryRepository _categoryRepository;

        public TransactionRepositoryImpl(AppDbContext context, ICategoryRepository categoryRepository = null)
        {
            _dbContext = context;
            _categoryRepository = categoryRepository;   
        }

        public async Task<TransactionEntity> AddCategoryToTransaction(string id, string catCode)
        {
            var transactionEntity = await Get(id);
            var categoryEntity = await _categoryRepository.FindByCode(catCode);

            transactionEntity.CatCode = catCode;
            transactionEntity.Category = categoryEntity;

            categoryEntity.Transactions ??= new();

            categoryEntity.Transactions.Add(transactionEntity);

            await _dbContext.SaveChangesAsync();

            return transactionEntity;

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

        

        public async Task<TransactionPagedList<TransactionEntity>> GetTransactionsAsQueryable(SearchParams searchParams)
        {

            // Between Date Filter
            IQueryable<TransactionEntity> transactions = _dbContext.Transactions
                .Where(t => t.Date >= searchParams.startDate && t.Date <= searchParams.endDate);

            // Transaction Kinds Filter
            if (searchParams.kinds != null && searchParams.kinds.Any())
            {
                transactions = transactions.Where(t => searchParams.kinds.Contains(t.Kind));
            }

            // Sorting
            if(!string.IsNullOrEmpty(searchParams.sortBy))
            {
                switch (searchParams.sortBy)
                {
                    case "date":
                        transactions = searchParams.sortOrder == SortOrder.Asc ?
                            transactions.OrderBy(t => t.Date) : transactions.OrderByDescending(t => t.Date);

                        break;

                    case "id":
                        transactions = searchParams.sortOrder == SortOrder.Asc ?
                            transactions.OrderBy(t => t.Id) : transactions.OrderByDescending(t => t.Id);

                        break;

                    case "beneficiaryName":
                        transactions = searchParams.sortOrder == SortOrder.Asc ?
                            transactions.OrderBy(t => t.BeneficiaryName) : transactions.OrderByDescending(t => t.BeneficiaryName);

                        break;

                    case "amount":
                        transactions = searchParams.sortOrder == SortOrder.Asc ?
                            transactions.OrderBy(t => t.Amount) : transactions.OrderByDescending(t => t.Amount);

                        break;

                    case "currency":
                        transactions = searchParams.sortOrder == SortOrder.Asc ?
                            transactions.OrderBy(t => t.Currency) : transactions.OrderByDescending(t => t.Currency);

                        break;

                    case "kind":
                        transactions = searchParams.sortOrder == SortOrder.Asc ?
                            transactions.OrderBy(t => t.Kind) : transactions.OrderByDescending(t => t.Kind);

                        break;

                    case "mcc":
                        transactions = searchParams.sortOrder == SortOrder.Asc ?
                            transactions.OrderBy(t => t.Mcc) : transactions.OrderByDescending(t => t.Mcc);

                        break;

                    default:
                        transactions = searchParams.sortOrder == SortOrder.Asc ?
                            transactions.OrderBy(t => t.Id) : transactions.OrderByDescending(t => t.Id);
                        break;
                }
            }

            else { transactions = transactions.OrderBy(t => t.Id); }

            int totalCount = transactions.Count();
            int totalPages = (int)Math.Ceiling(totalCount * 1.0 / searchParams.pageSize);


            transactions = transactions.Skip((searchParams.page - 1) * searchParams.pageSize)
                               .Take(searchParams.pageSize);


            //return await transactions.ToListAsync();

            return new TransactionPagedList<TransactionEntity>
            {
                Page = searchParams.page,
                PageSize = searchParams.pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                SortBy = searchParams.sortBy,
                SortOrder = searchParams.sortOrder,
                StartDate = searchParams.startDate,
                EndDate = searchParams.endDate,
                Kinds = searchParams.kinds,
                Items = await transactions.ToListAsync()

            };
        }
    }
 }

