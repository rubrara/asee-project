using Microsoft.EntityFrameworkCore;
using PFMdotnet.Database.Entities;
using PFMdotnet.Helpers;
using PFMdotnet.Models;
using System.Net;
using System.Web.Http;

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

        public async Task<ReturnDTO<TransactionEntity>> AddCategoryToTransaction(string id, string catCode)
        {

            List<string> errors = new();

            TransactionEntity transactionEntity = await Get(id);
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
                return new ReturnDTO<TransactionEntity>
                {
                    Process = string.Format("Failed to add Category: '{0}' to Transacton: '{1}'", catCode, id),
                    Errors = errors
                };
            }  

            transactionEntity.CatCode = catCode;
            transactionEntity.Category = categoryEntity;

            categoryEntity.Transactions ??= new();

            categoryEntity.Transactions.Add(transactionEntity);

            await _dbContext.SaveChangesAsync();

            return new ReturnDTO<TransactionEntity>
            {
                Process = string.Format("Adding Category: '{0}' to Transacton: '{1}'", catCode, id),
                Value = transactionEntity
            };

        }

        public async Task<TransactionEntity> Create(TransactionEntity transaction)
        {
            _dbContext.Transactions.Add(transaction);

            await _dbContext.SaveChangesAsync();

            return transaction;
        }

        public async Task<AfterBulkAdd<TransactionEntity>> CreateBulk(List<TransactionEntity> transactions)
        {
            int chunkSize = 500;
            int total = transactions.Count;
            var chunks = ListToChunks.ChunkList(transactions, chunkSize);

            try
            {
                foreach (var chunk in chunks)
                {
                    await _dbContext.Transactions.AddRangeAsync(chunk);
                    await _dbContext.SaveChangesAsync();
                }


                return new AfterBulkAdd<TransactionEntity>
                {
                    toAdd = "Transactions",
                    totalRowsAdded = total,
                    rowExample = transactions.ElementAt(0)
                };

            } catch (Exception)
            {
                return new AfterBulkAdd<TransactionEntity>
                {
                    toAdd = "Transactions",
                    totalRowsAdded = 0,
                    Error = "There was an error trying to add the transactions!"
                };
            }
            
        }

        public async Task<TransactionEntity> Get(string Id)
        {

            var res = await _dbContext.Transactions.FirstOrDefaultAsync(p => p.Id == Id);

            return res;
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
                        searchParams.sortBy = "id";
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

