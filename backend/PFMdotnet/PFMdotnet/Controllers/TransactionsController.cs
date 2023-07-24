using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using PFMdotnet.Commands;
using PFMdotnet.Database.Enums;
using PFMdotnet.Helpers.ParseCSV;
using PFMdotnet.Models;
using PFMdotnet.Services;
using System.Globalization;
using System.Linq.Expressions;

namespace PFMdotnet.Controllers
{

    [ApiController]
    [Route("transactions")]
    public class TransactionsController : ControllerBase
    {

        private readonly ITransactionService _transactionService;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(ITransactionService transactionService, ILogger<TransactionsController> logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactions(
            [FromQuery] int? page, 
            [FromQuery] int? pageSize, 
            [FromQuery] string? sortBy, 
            [FromQuery] SortOrder? sortOrder,
            [FromQuery] string? startDate,
            [FromQuery] string? endDate,
            [FromQuery] string? kindsString
            )
        {

            List<KindEnum>? kinds = new();

            if (!string.IsNullOrEmpty(kindsString))
            {
                string[] kindsArray = kindsString.Split(',');

                foreach (string kind in kindsArray)
                {
                    try
                    {
                        kinds.Add(Enum.Parse<KindEnum>(kind));
                    }
                    catch(Exception)
                    {
                        return BadRequest(new
                        {
                            Message = "A Transaction Kind you entered is Invalid"
                        });
                    }
                }
            } else
            {
                kinds = Enum.GetValues(typeof(KindEnum)).Cast<KindEnum>().ToList();
            }

            page = page > 0 ? page : 1;
            pageSize = (pageSize < 1 || pageSize == null) ? 5 : pageSize > 50 ? 50 : pageSize;

            _logger.LogInformation("Returning {page}. page of transactions", page);
         
            var result = await _transactionService.GetTransactionsAsQueriable(new SearchParams()
            {
                page = (int)page,
                pageSize = (int)pageSize,
                sortBy = sortBy,
                sortOrder = (SortOrder)(sortOrder != null ? sortOrder : SortOrder.Asc),
                startDate = string.IsNullOrEmpty(startDate) ? DateOnly.MinValue : DateOnly.Parse(startDate),
                endDate = string.IsNullOrEmpty(endDate) ? DateOnly.MaxValue : DateOnly.Parse(endDate),
                kinds = kinds
            });

            return Ok(result);
        }

        [HttpPost]
        [Route("import")]
        public async Task<IActionResult> ImportTransactions(IFormFile file)
        {

            var res = await _transactionService.CreateTransactionBulk(CsvParse<CreateTransactionCommand>.ToList(file));

            return Ok(res);
        }

        [HttpPost]
        [Route("{id}/categorize")]
        public async Task<IActionResult> CategorizeTransaction([FromRoute] string id, [FromQuery] string catCode)
        {
            
            var result = await _transactionService.AddCategoryToTransaction(id, catCode);

            if (result.Errors.Count != 0)
            {
                Console.WriteLine(result.Errors.Count);
                return NotFound(result);
            }

            result.Errors.Add("None");

            return Ok(result);   

        }




    }
}
