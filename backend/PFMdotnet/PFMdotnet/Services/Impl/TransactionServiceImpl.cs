using AutoMapper;
using Newtonsoft.Json;
using PFMdotnet.Commands;
using PFMdotnet.Database;
using PFMdotnet.Database.Entities;
using PFMdotnet.Database.Enums;
using PFMdotnet.Database.Repositories;
using PFMdotnet.Helpers;
using PFMdotnet.Helpers.SearchReturnObjects;
using PFMdotnet.Helpers.SearchReturnObjects.Transactions;
using PFMdotnet.Helpers.Validation;
using PFMdotnet.Models;
using PFMdotnet.Models.Rules;

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

            List<string> errors = new();

            var returnDto = new ReturnDTO<TransactionDto>();

            if (string.IsNullOrEmpty(catCode))
            {
                errors.Add("You have not given category code!");
            }

            Transaction transactionEntity = await _transactionRepository.Get(id);  
            if (transactionEntity == null)
            {
                errors.Add(string.Format("The Transaction Id: {0} doesn't exist", id));
            };


            var categoryEntity = await _categoryRepository.FindByCode(catCode); 
            if (categoryEntity == null)
            {
                errors.Add(string.Format("The Category Code: {0} doesn't exist", catCode));
            }

            if (errors.Count != 0)
            {
                returnDto.Message = string.Format("Failed to add Category: '{0}' to Transacton: '{1}'", catCode, id);
                returnDto.Errors = errors;

                return returnDto;
            }

            transactionEntity.CatCode = catCode;
            transactionEntity.Category = categoryEntity;

            categoryEntity.Transactions ??= new();

            categoryEntity.Transactions.Add(transactionEntity);
            await _transactionRepository.SaveChangesAsync();

            return new ReturnDTO<TransactionDto>
            {
                Message = string.Format("Adding Category: '{0}' to Transacton: '{1}'", catCode, id),
                Result = _mapper.Map<TransactionDto>(transactionEntity)
            };


        }

        public async Task<ReturnDTO<List<TransactionDto>>> AddCategoryToManyTransactions(string ids, string catCode)
        {
            var idList = new List<string>();

            if (!string.IsNullOrEmpty(ids))
            { 
                char[] delimiters = { ' ', ',' };

                idList = ids.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            List<string> errors = new();
            var returnDto = new ReturnDTO<List<TransactionDto>>();

            if (string.IsNullOrEmpty(catCode))
            {
                errors.Add("You have not given category code!");
            }
            

            if (!idList.Any())
            {

                errors.Add("You have not given any transactions!");

                return new ReturnDTO<List<TransactionDto>>
                {
                    Message = "Failed to assign category to transactions",
                    Errors = errors
                };

            }
            var categoryEntity = await _categoryRepository.FindByCode(catCode);

            if (categoryEntity == null)
            {
                errors.Add(string.Format("The category code: '{0}' does not exist", catCode));

                return new ReturnDTO<List<TransactionDto>>
                {
                    Message = "Failed to assign category to transactions",
                    Errors = errors
                };

            }

            List<TransactionDto> transactionList = new List<TransactionDto>();

            foreach (var id in idList)
            {
                Transaction transactionEntity = await _transactionRepository.Get(id);
                if (transactionEntity == null)
                {
                    errors.Add(string.Format("The Transaction Id: {0} doesn't exist", id));
                }
                else
                {

                    transactionEntity.CatCode = catCode;
                    transactionEntity.Category = categoryEntity;

                    categoryEntity.Transactions ??= new();

                    categoryEntity.Transactions.Add(transactionEntity);

                    transactionList.Add(_mapper.Map<TransactionDto>(transactionEntity));
                }
            }

            if (errors.Any())
            {

                return new ReturnDTO<List<TransactionDto>>
                {
                    Message = "Failed to assign category to transactions",
                    Errors = errors
                };
            }

            await _transactionRepository.SaveChangesAsync();


            returnDto.Result = transactionList;
            returnDto.Message = "Successfuly added Category for all the given Transactons";

            return returnDto;

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

            res.Result = _mapper.Map<TransactionDto>(transactionEntity);

            return res;
        }

        public async Task<TransactionPagedList<TransactionDto>> GetTransactionsAsQueriable(SearchTransactionParams searchParams)
        {

            var searchTransactionParams = Validate.ValidateTransactionFilterParams(searchParams);

            if (searchTransactionParams.Errors != null)
            {
                return new TransactionPagedList<TransactionDto>
                {
                    Message = "Filter transactions is not allowed!",
                    Errors = searchTransactionParams.Errors
                };
            }

            var result = await _transactionRepository.GetTransactionsAsQueryable(searchTransactionParams);

            foreach(var item in result.Items)
            {
                if (!item.Splits.Any())
                {
                    item.Splits = null;
                }
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

        public async Task<CategorizationReturn> AutoCategorize()
        {
            string json = File.ReadAllText("C:\\Users\\koki_\\source\\repos\\asee-project\\backend\\PFMdotnet\\PFMdotnet\\rules.json");
            var _rules = JsonConvert.DeserializeObject<List<CategorizationRule>>(json);

            var result = new CategorizationReturn();
            var total = await (_transactionRepository.AutoCategorize(_rules));

            if (total == -1)
            {
                return new CategorizationReturn()
                {
                    Message = "Could not auto-categorize fully",
                    Error = "Something went wrong when doing the query"
                };
            }

            if (total > 0)
            {
                return new CategorizationReturn()
                {
                    Message = string.Format("Successful autocategorization of {0} transactions", total)
                };
            }

            return new CategorizationReturn()
            {
                Message = "Could not auto-categorize any transactions. No good rules"
            };

            
        }
    }
}
