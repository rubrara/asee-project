using AutoMapper;
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
    }
}
