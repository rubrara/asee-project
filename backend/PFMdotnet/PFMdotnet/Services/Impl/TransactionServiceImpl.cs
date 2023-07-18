using AutoMapper;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PFMdotnet.Commands;
using PFMdotnet.Database.Entities;
using PFMdotnet.Database.Repositories;
using PFMdotnet.Mappings;
using PFMdotnet.Models;

namespace PFMdotnet.Services.Impl
{
    public class TransactionServiceImpl : ITransactionService
    {

        private readonly IMapper _mapper;
        private readonly ITransactionRepository _transactionRepository;
        public TransactionServiceImpl(IMapper mapper, ITransactionRepository transactionRepository) {
            _mapper = mapper;
            _transactionRepository = transactionRepository;
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

        public async Task<List<Transaction>> CreateTransactionBulk(List<CreateTransactionCommand> commands)
        {

            var entities = _mapper.Map<List<TransactionEntity>>(commands);

            if (entities == null)
            {
                return null;
            }

         

            var result = await _transactionRepository.CreateBulk(entities);

            return _mapper.Map<List<Transaction>>(entities);
        }

        public async Task<Transaction> GetProduct(string transactionCode)
        {
            var transactionEntity = await _transactionRepository.Get(transactionCode);

            if (transactionEntity == null)
            {
                return null;
            }

            return _mapper.Map<Models.Transaction>(transactionEntity);
        }

        public async Task<PagedSortedList<Transaction>> GetTransactions(int page = 1, int pageSize = 10)
        {
            var result = await _transactionRepository.List(page, pageSize);

            return _mapper.Map<PagedSortedList<Models.Transaction>>(result);
        }
    }
}
