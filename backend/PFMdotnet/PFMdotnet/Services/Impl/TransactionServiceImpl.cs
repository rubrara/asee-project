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

        public async Task<ReturnDTO<TransactionDto>> AddCategoryToTransaction(string id, string catCode)
        {

            var result = await _transactionRepository.AddCategoryToTransaction(id, catCode);

            return _mapper.Map<ReturnDTO<TransactionDto>>(result);


        }

        public async Task<TransactionDto> CreateTransaction(CreateTransactionCommand command)
        {
            var entity = _mapper.Map<Transaction>(command);

            var existingProduct = await _transactionRepository.Get(command.Id);
            if (existingProduct != null)
            {
                return null;
            }
            var result = await _transactionRepository.Create(entity);

            return _mapper.Map<TransactionDto>(result);
        }

        public async Task<AfterBulkAdd<Transaction>> CreateTransactionBulk(List<CreateTransactionCommand> commands)
        {

            var entities = _mapper.Map<List<Transaction>>(commands);

            if (entities == null)
            {
                return null;
            }

            int chunkSize = 200;
            var result = await _transactionRepository.CreateBulk(entities, chunkSize);

            return result;
        }

        public async Task<ReturnDTO<TransactionDto>> GetTransaction(string transactionCode)
        {
            var transactionEntity = await _transactionRepository.Get(transactionCode);

            var res = new ReturnDTO<TransactionDto>()
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

            res.Value = _mapper.Map<TransactionDto>(transactionEntity);

            return res;
        }

        public async Task<TransactionPagedList<Transaction>> GetTransactionsAsQueriable(SearchTransactionParams searchParams)
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
                return new TransactionPagedList<Transaction>
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
                var res = new TransactionPagedList<Transaction>
                {
                    Message = "Import of transactions is not allowed!",
                    Errors = new()
                };           

                res.Errors.Add(string.Format("You can NOT access page: {0} since there are {1} total pages!", page, result.TotalPages));

                return res;
            }

            return result;

        }

        public async Task<ReturnDTO<List<TransactionDto>>> SplitTransactionAsync(string transactionId, SplitByParams parameters)
        {

            var transaction = await _transactionRepository.Get(transactionId);
            double sum = 0;

            ReturnDTO<List<TransactionDto>> returnDto = new()
            {
                Message = "Split Transaction",
            };
            string error;

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

            if (parameters.Splits.Count < 2)
            {
                returnDto.Message = "You need to have two or more splits to split a transaction";
                return returnDto;
            }

            if (transaction.Amount != parameters.Splits.Sum(split => split.Amount))
            {
                error = "The amounts sum of the Splits is not the same with the amount of the Transaction";

                returnDto.Errors = new() { error };

                return returnDto;
            }

            List<TransactionSplit> splits = new();

            foreach (var split in parameters.Splits)
            {
                // za sekoj split treba da se napraj nov TransactionSplit so nekakov id
                // Toj split treba da se dodaj vo TransactionEntity vo Splits listata

                if ((await _categoryRepository.FindByCode(split.CatCode) == null))
                {
                    returnDto.Errors = new()
                    {
                        string.Format("The category with code: {0} doesn't exist", split.CatCode)
                    };

                    return returnDto;
                }

                var splitTransactionEntity = new TransactionSplit
                {
                    Id = Guid.NewGuid(),
                    TransactionId = transaction.Id,
                    CatCode = split.CatCode,
                    Amount = split.Amount
                };

                splits.Add(splitTransactionEntity);
            }

            if (transaction.Splits != null)
            {
                await _transactionRepository.DeleteTransactionSplits(transaction);
            }

            transaction.Splits = splits;

            if(await _transactionRepository.AddTransactionSplits(splits))
            {
                returnDto.Message = "Sucessfuly splited the transaction!";

                return returnDto;
            } else
            {
                returnDto.Errors = new()
                    {
                        "There was a problem in storing the splits in the database"
                    };

                return returnDto;
            }


            
        }
    }
}
