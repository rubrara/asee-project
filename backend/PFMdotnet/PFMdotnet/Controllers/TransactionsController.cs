using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using PFMdotnet.Commands;
using PFMdotnet.Database.Enums;
using PFMdotnet.Helpers.ParseCSV;
using PFMdotnet.Models;
using PFMdotnet.Services;
using System.Globalization;

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
            [FromQuery] SortOrder sortOrder,
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
                    kinds.Add(Enum.Parse<KindEnum>(kind));
                }
            } else
            {
                kinds = Enum.GetValues(typeof(KindEnum)).Cast<KindEnum>().ToList();
            }

            _logger.LogInformation("Returning {page}. page of transactions", page);

            SearchParams searchParams = new();

            searchParams.page = page ?? 1;
            searchParams.pageSize = pageSize ?? 10;
            searchParams.sortBy = sortBy;
            searchParams.sortOrder = sortOrder;
            searchParams.startDate = string.IsNullOrEmpty(startDate) ? DateOnly.MinValue : DateOnly.Parse(startDate);
            searchParams.endDate = string.IsNullOrEmpty(endDate) ? DateOnly.MaxValue : DateOnly.Parse(endDate);
            searchParams.kinds = kinds;

            var result = await _transactionService.GetTransactionsAsQueriable(searchParams);

            return Ok(result);
        }

        [HttpPost]
        [Route("import")]
        public async Task<IActionResult> ImportTransactions(IFormFile file)
        {

            await _transactionService.CreateTransactionBulk(CsvParse<CreateTransactionCommand>.ToList(file));

            return Ok("Transactions imported successfully.");
        }

        [HttpPost]
        [Route("{id}/categorize")]
        public async Task<IActionResult> CategorizeTransaction([FromRoute] string id, [FromQuery] string catCode)
        {
            try {
                var result = await _transactionService.AddCategoryToTransaction(id, catCode);
                return Ok(result);
            } catch (Exception ex)
            {
                return BadRequest("The transaction ID and/or Caterory Code does not exist!");
            }
            

            

        }




    }
}
