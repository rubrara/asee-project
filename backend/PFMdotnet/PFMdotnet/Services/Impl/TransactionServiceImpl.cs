using AutoMapper;

using PFMdotnet.Commands;
using PFMdotnet.Database.Entities;
using PFMdotnet.Database.Enums;
using PFMdotnet.Database.Repositories;
using PFMdotnet.Helpers;
using PFMdotnet.Helpers.SearchReturnObjects.Transactions;
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

            return _mapper.Map<Transaction>(result);
        }

        public async Task<AfterBulkAdd<TransactionEntity>> CreateTransactionBulk(List<CreateTransactionCommand> commands)
        {

            var entities = _mapper.Map<List<TransactionEntity>>(commands);

            if (entities == null)
            {
                return null;
            }

            int chunkSize = 200;
            var result = await _transactionRepository.CreateBulk(entities, chunkSize);

            return result;
        }

        public async Task<ReturnDTO<Transaction>> GetTransaction(string transactionCode)
        {
            var transactionEntity = await _transactionRepository.Get(transactionCode);

            var res = new ReturnDTO<Transaction>()
            {
                Message = string.Format("Getting Transaction for Id: {0}", transactionCode)
            };

            if (transactionEntity == null)
            {
                res.Errors = new()
                {
                    string.Format("The database doesnt contain transaction by Id: {0}", transactionCode)
                };

                return res;
            }

            res.Value = _mapper.Map<Transaction>(transactionEntity);

            return res;
        }

        public async Task<TransactionPagedList<TransactionEntity>> GetTransactionsAsQueriable(SearchTransactionParams searchParams)
        {

            List<KindEnum>? kinds = new();
            List<string> errors = new();

            if (!string.IsNullOrEmpty(searchParams.Kinds))
            {
                char[] delimiters = { ' ', ','};
                string[] kindsArray = searchParams.Kinds.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                foreach (string kind in kindsArray)
                {
                    try
                    {
                        kinds.Add(Enum.Parse<KindEnum>(kind));
                    }
                    catch (Exception)
                    {
                        errors.Add(string.Format("'{0}' is NOT a valid transaction kind", kind));
                    }
                }
            }
            else
            {
                kinds = Enum.GetValues(typeof(KindEnum)).Cast<KindEnum>().ToList();
            }

            int page = searchParams.Page > 0 ? (int) searchParams.Page : 1;
            int pageSize = (searchParams.PageSize < 1 || searchParams.PageSize == null) ? 5 : searchParams.PageSize > 50 ? 50 : (int) searchParams.PageSize;

            DateOnly startDate;
            DateOnly endDate;

            if (string.IsNullOrEmpty(searchParams.StartDate))
            {
                startDate = DateOnly.MinValue;

            }
            else if (!DateOnly.TryParse(searchParams.StartDate, out startDate))
            {
                errors.Add(string.Format("Invalid date format for the StartDate: '{0}'. Please provide a valid date in the format 'YYYY-MM-DD'.", searchParams.StartDate));

            }

            if (string.IsNullOrEmpty(searchParams.EndDate))
            {
                endDate = DateOnly.MaxValue;

            }
            else if (!DateOnly.TryParse(searchParams.EndDate, out endDate))
            {
                errors.Add(string.Format("Invalid date format for the EndDate: '{0}'. Please provide a valid date in the format 'YYYY-MM-DD'.", searchParams.EndDate));
            }

            SortOrder sortOrder;

            if(string.IsNullOrEmpty(searchParams.SortOrder)) {
                sortOrder = SortOrder.Asc;
            }
            else if (!Enum.TryParse(searchParams.SortOrder, true, out sortOrder))
            {
                errors.Add(string.Format("'{0}' is NOT part of the SortOrder enum.", searchParams.SortOrder));
            }



            if (errors.Any())
            {
                return new TransactionPagedList<TransactionEntity>
                {
                    Message = "Import of transactions is not allowed!",
                    Errors = errors
                };
            }


            var searchTransactionParams = new FilterTransactionsParams()
            {
                Page = page,
                PageSize = pageSize,
                SortBy = searchParams.SortBy,
                SortOrder = sortOrder,
                StartDate = startDate,
                EndDate = endDate,
                Kinds = kinds
            }; 

            var result = await _transactionRepository.GetTransactionsAsQueryable(searchTransactionParams);

            if (page > result.TotalPages)
            {
                var res = new TransactionPagedList<TransactionEntity>
                {
                    Message = "Import of transactions is not allowed!",
                    Errors = new()
                };           

                res.Errors.Add(string.Format("You can NOT access page: {0} since there are {1} total pages!", page, result.TotalPages));

                return res;
            }

            return result;

        }

        public async Task<ReturnDTO<List<Transaction>>> SplitTransactionAsync(string transactionId, SplitByParams parameters)
        {

            ReturnDTO<List<Transaction>> returnDto = new()
            {
                Message = "Trying to Split a Transaction",
            };
            string error;

            var transaction = _mapper.Map<Transaction>(await _transactionRepository.Get(transactionId));
            if (transaction == null)
            {
                error = string.Format("The Transaction with id: {0} doesn't exist", transactionId);

                returnDto.Errors = new() { error };

                return returnDto;
            }

            // treba da se podeli transakcijata spored parameters.catcodes
            // treba da se podeli na parameters.count splits
            // ako ne postoj parameters.CatCode hendlaj errors
            // ako sumata na amaounts od splits != transaction.amount hendlaj errors

            // Check if the sum is equal with the transaction amount
            if (transaction.Amount != parameters.Splits.Sum(split => split.Amount))
            {
                error = "The amounts sum of the Splits is not the same with the amount of the Transaction";

                returnDto.Errors = new() { error };

                return returnDto;
            }

            foreach (var split in parameters.Splits)
            {
                // do stuff gtg ujp
            }
            

            throw new NotImplementedException();
        }
    }
}
