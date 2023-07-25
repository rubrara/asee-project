using Microsoft.EntityFrameworkCore;
using PFMdotnet.Database.Entities;
using PFMdotnet.Helpers;
using PFMdotnet.Helpers.SearchReturnObjects.Transactions;
using PFMdotnet.Models;

namespace PFMdotnet.Database.Repositories.Impl
{
    public class TransactionRepositoryImpl : ITransactionRepository
    {

        private readonly AppDbContext _dbContext;
        private readonly ICategoryRepository _categoryRepository;

        public TransactionRepositoryImpl(AppDbContext context, ICategoryRepository categoryRepository)
        {
            _dbContext = context;
            _categoryRepository = categoryRepository;   
        }

        public async Task<ReturnDTO<Transaction>> AddCategoryToTransaction(string id, string catCode)
        {

            List<string> errors = new();

            Transaction transactionEntity = await Get(id);
            if (transactionEntity == null)
            {
                errors.Add(string.Format("The Transaction Id: {0} doesn't exist", id));
            };


            var categoryEntity = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Code.Equals(catCode));
            if (categoryEntity == null) {
                errors.Add(string.Format("The Category Code: {0} doesn't exist", catCode));
            }

            if (errors.Count != 0)
            {
                return new ReturnDTO<Transaction>
                {
                    Message = string.Format("Failed to add Category: '{0}' to Transacton: '{1}'", catCode, id),
                    Errors = errors
                };
            }  

            transactionEntity.CatCode = catCode;
            transactionEntity.Category = categoryEntity;

            categoryEntity.Transactions ??= new();

            categoryEntity.Transactions.Add(transactionEntity);

            await _dbContext.SaveChangesAsync();

            return new ReturnDTO<Transaction>
            {
                Message = string.Format("Adding Category: '{0}' to Transacton: '{1}'", catCode, id),
                Value = transactionEntity
            };

        }

        public async Task DeleteTransactionSplits(Transaction transaction)
        {
            var toDelete = _dbContext.Splits.Where(s => s.TransactionId.Equals(transaction.Id));

            _dbContext.Splits.RemoveRange(toDelete);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> AddTransactionSplits(List<TransactionSplit> splits)
        {
            try
            {
                _dbContext.Splits.AddRange(splits);

                await _dbContext.SaveChangesAsync();

                return true;

        } catch
            {
                return false;
            }
            
        }

        public async Task<Transaction> Create(Transaction transaction)
        {
            _dbContext.Transactions.Add(transaction);

            await _dbContext.SaveChangesAsync();

            return transaction;
        }

        public async Task<AfterBulkAdd<Transaction>> CreateBulk(List<Transaction> transactions, int chunkSize)
        {
            int total = transactions.Count;
            int totalAdded = 0;
            int totalUpdated = 0;

            var uniqueIds = transactions.Select(t => t.Id).ToList();

            var existingTransactions = _dbContext.Transactions
                .Where(t => uniqueIds.Contains(t.Id))
                .ToDictionary(t => t.Id);

            var newTransactions = transactions.Where(t => !existingTransactions.ContainsKey(t.Id)).ToDictionary(t => t.Id);

            for (int i = 0; i < total; i += chunkSize)
            {

                var chunkNew = newTransactions.Values.ToList().Skip(i).Take(chunkSize).ToList();
                var chunkExisting = existingTransactions.Values.ToList().Skip(i).Take(chunkSize).ToList();

                if (chunkNew.Any())
                {
                    await _dbContext.Transactions.AddRangeAsync(chunkNew);
                    totalAdded += chunkNew.Count();
                }

                if (chunkExisting.Any())
                {
                    _dbContext.Transactions.UpdateRange(chunkExisting);
                    totalUpdated += chunkExisting.Count();
                }

                await _dbContext.SaveChangesAsync();
            }

            return new AfterBulkAdd<Transaction> { 
                Message = "Uploading Transactions form CSV file",
                TotalRowsAdded = totalAdded == 0 ? null : totalAdded, 
                TotalRowsUpdated = totalUpdated == 0 ? null : totalUpdated,
            };
            
        }

        public async Task<Transaction> Get(string Id)
        {

            var res = await _dbContext.Transactions.Where(t => t.Id.Equals(Id)).Include(t => t.Splits).FirstOrDefaultAsync(p => p.Id == Id);

            return res;
        }

        

        public async Task<TransactionPagedList<Transaction>> GetTransactionsAsQueryable(FilterTransactionsParams searchParams)
        {

            // Between Date Filter
            IQueryable<Transaction> transactions = _dbContext.Transactions
                .Where(t => t.Date >= searchParams.StartDate && t.Date <= searchParams.EndDate);

            // Transaction Kinds Filter
            if (searchParams.Kinds != null && searchParams.Kinds.Any())
            {
                transactions = transactions.Where(t => searchParams.Kinds.Contains(t.Kind));
            }

            // Sorting
            if(!string.IsNullOrEmpty(searchParams.SortBy))
            {
                switch (searchParams.SortBy)
                {
                    case "date":
                        transactions = searchParams.SortOrder == SortOrder.Asc ?
                            transactions.OrderBy(t => t.Date) : transactions.OrderByDescending(t => t.Date);

                        break;

                    case "id":
                        transactions = searchParams.SortOrder == SortOrder.Asc ?
                            transactions.OrderBy(t => t.Id) : transactions.OrderByDescending(t => t.Id);

                        break;

                    case "beneficiaryName":
                        transactions = searchParams.SortOrder == SortOrder.Asc ?
                            transactions.OrderBy(t => t.BeneficiaryName) : transactions.OrderByDescending(t => t.BeneficiaryName);

                        break;

                    case "amount":
                        transactions = searchParams.SortOrder == SortOrder.Asc ?
                            transactions.OrderBy(t => t.Amount) : transactions.OrderByDescending(t => t.Amount);

                        break;

                    case "currency":
                        transactions = searchParams.SortOrder == SortOrder.Asc ?
                            transactions.OrderBy(t => t.Currency) : transactions.OrderByDescending(t => t.Currency);

                        break;

                    case "kind":
                        transactions = searchParams.SortOrder == SortOrder.Asc ?
                            transactions.OrderBy(t => t.Kind) : transactions.OrderByDescending(t => t.Kind);

                        break;

                    case "mcc":
                        transactions = searchParams.SortOrder == SortOrder.Asc ?
                            transactions.OrderBy(t => t.Mcc) : transactions.OrderByDescending(t => t.Mcc);

                        break;

                    default:
                        transactions = searchParams.SortOrder == SortOrder.Asc ?
                            transactions.OrderBy(t => t.Id) : transactions.OrderByDescending(t => t.Id);
                        searchParams.SortBy = "id";
                        break;
                }
            }

            else { transactions = transactions.OrderBy(t => t.Id); }

            int pageSize = (int)searchParams.PageSize;

            int totalCount = transactions.Count();
            int totalPages = (int)Math.Ceiling(totalCount * 1.0 / searchParams.PageSize);


            transactions = transactions.Skip((searchParams.Page - 1) * searchParams.PageSize)
                               .Take(searchParams.PageSize);


            //return await transactions.ToListAsync();

            return new TransactionPagedList<Transaction>
            {
                Page = searchParams.Page,
                PageSize = searchParams.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                SortBy = searchParams.SortBy,
                SortOrder = searchParams.SortOrder,
                StartDate = searchParams.StartDate,
                EndDate = searchParams.EndDate,
                Kinds = searchParams.Kinds,
                Items = await transactions.ToListAsync()

            };
        }
    }
 }

