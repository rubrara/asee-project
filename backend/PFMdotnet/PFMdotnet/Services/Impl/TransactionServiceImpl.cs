using AutoMapper;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PFMdotnet.Commands;
using PFMdotnet.Database.Entities;
using PFMdotnet.Database.Repositories;
using PFMdotnet.Helpers.Mappings;
using PFMdotnet.Models;

namespace PFMdotnet.Services.Impl
{
    public class TransactionServiceImpl : ITransactionService
    {

        private readonly IMapper _mapper;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICategoryRepository _categoryRepository;
        public TransactionServiceImpl(IMapper mapper, ITransactionRepository transactionRepository, ICategoryRepository categoryRepository)
        {
            _mapper = mapper;
            _transactionRepository = transactionRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<ReturnDTO<Transaction>> AddCategoryToTransaction(string id, string catCode)
        {

            var result = await _transactionRepository.AddCategoryToTransaction(id, catCode);

            return _mapper.Map<ReturnDTO<Transaction>>(result);


        }

        public async Task<Transaction> CreateTransaction(CreateTransactionCommand command)
        {
            var entity = _mapper.Map<TransactionEntity>(command);

            var existingProduct = await _transactionRepository.Get(command.Id);
            if (existingProduct != null)
            {
                return null;
            }
            var result = await _transactionRepository.Create(entity);

            return _mapper.Map<Models.Transaction>(result);
        }

        public async Task<AfterBulkAdd<TransactionEntity>> CreateTransactionBulk(List<CreateTransactionCommand> commands)
        {

            var entities = _mapper.Map<List<TransactionEntity>>(commands);

            if (entities == null)
            {
                return null;
            }

         

            var result = await _transactionRepository.CreateBulk(entities);

            return result;
        }

        public async Task<Transaction> GetTransaction(string transactionCode)
        {
            var transactionEntity = await _transactionRepository.Get(transactionCode);

            if (transactionEntity == null)
            {
                return null;
            }

            return _mapper.Map<Transaction>(transactionEntity);
        }

        public async Task<TransactionPagedList<TransactionEntity>> GetTransactionsAsQueriable(SearchParams searchParams)
        {
            var result = await _transactionRepository.GetTransactionsAsQueryable(searchParams);

            return result;

        }
    }
}
