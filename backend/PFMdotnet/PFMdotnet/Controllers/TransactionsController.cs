using Microsoft.AspNetCore.Mvc;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PFMdotnet.Services;
using System.Collections.Generic;

namespace PFMdotnet.Controllers
{

    [ApiController]
    [Route("/transactions")]
    [Route("/")]
    public class TransactionsController : ControllerBase
    {

        private readonly ITransactionService _transactionService;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(ITransactionService transactionService, ILogger<TransactionsController> logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }


        /* 
         * Transaction list should be filtered by transaction kind.
         * Transaction list should be sorted by date (descending) and category (ascending).
         * 
         * Implement period filter (start-date and end-date).
         * Implement transaction kinds filter as a list of acceptable transaction kinds.
        */

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] int? page, [FromQuery] int? pageSize)
        {
            page = page ?? 1;
            pageSize = pageSize ?? 9;
            _logger.LogInformation("Returning {page}. page of products", page);
            var result = await _transactionService.GetTransactions(page.Value, pageSize.Value);
            return Ok(result);
        }
    }
}
